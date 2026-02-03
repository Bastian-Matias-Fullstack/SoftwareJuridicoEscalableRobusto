using Aplicacion.DTOs;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Dominio.Entidades;

namespace Aplicacion.Servicios.Casos
{
    public class ActualizarCasoService
    {
        private readonly ICasoRepository _casoRepository;
        private readonly IClienteRepository _clienteRepository;
        public ActualizarCasoService(
            ICasoRepository casoRepository,
            IClienteRepository clienteRepository
            )
        {
            _casoRepository = casoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task EjecutarAsync(int id, ActualizarCasoRequest request, bool esAdmin)
        {
            var caso = await _casoRepository.ObtenerPorIdAsync(id);

            if (caso is null)
                throw new NotFoundException($"No existe un caso con id {id}.");
            if (request.ClienteId <= 0)
                throw new ArgumentException("Debe seleccionar un cliente válido.");

            request.Titulo = request.Titulo?.Trim() ?? string.Empty;
            request.Descripcion = request.Descripcion?.Trim() ?? string.Empty;
            //Regla dura: no editar cerrados
            // Regla dura: casos cerrados
            // Excepción: edición administrativa
            if (caso.Estado == EstadoCaso.Cerrado && !esAdmin)
                throw new BusinessConflictException(
                    "No se puede editar un caso cerrado."
                );

            // EN PROCESO → edición limitada (solo no admin)
            if (caso.Estado == EstadoCaso.EnProceso && !esAdmin)
            {
                caso.Descripcion = request.Descripcion;
                caso.UpdatedAt = DateTimeOffset.UtcNow;

                await _casoRepository.ActualizarAsync(caso);
                return;
            }


            //Cambio de cliente (solo si cambió)
            if (caso.ClienteId != request.ClienteId)
            {
                var cliente = await _clienteRepository.ObtenerPorIdAsync(request.ClienteId);

                if (cliente is null)
                    throw new NotFoundException("El cliente no existe.");

                var clienteTieneOtroCasoActivo =
               await _casoRepository.ExisteCasoActivoParaClienteAsync(
                   request.ClienteId,
                   caso.Id
               );

                if (clienteTieneOtroCasoActivo)
                    throw new BusinessConflictException(
                        "El cliente ya tiene otro caso activo."
                    );
                caso.ClienteId = request.ClienteId;       

            }
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new ArgumentException("El título del caso es obligatorio.");
            // Campos editables
            caso.Titulo = request.Titulo;
            caso.Descripcion = request.Descripcion;
            caso.TipoCaso = request.TipoCaso;
            caso.UpdatedAt = DateTimeOffset.UtcNow;

            // Regla automática de estado
            if (caso.Estado == EstadoCaso.Pendiente)
            {
                caso.Estado = EstadoCaso.EnProceso;
                caso.FechaCambioEstado = DateTime.UtcNow;
            }

            await _casoRepository.ActualizarAsync(caso);
        }
    }
}
