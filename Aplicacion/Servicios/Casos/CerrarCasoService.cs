using Aplicacion.DTO;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Dominio.Entidades;
using Microsoft.AspNetCore.Http;

namespace Aplicacion.Servicios.Casos
{
    public class CerrarCasoService
    {
        private readonly ICasoRepository _casoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CerrarCasoService(
            ICasoRepository casoRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _casoRepository = casoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task EjecutarAsync(int casoId, CerrarCasoRequest request)
        {
            // Validación de request
            if (request is null)
                throw new InvalidEstadoCasoException(
                    "La solicitud no contiene datos válidos para el cierre."
                );

            request.MotivoCierre = (request.MotivoCierre ?? string.Empty).Trim();

            // Auditoría
            var userName =
                _httpContextAccessor.HttpContext?.User?.Identity?.Name
                ?? "Sistema";

            // Obtener caso
            var caso = await _casoRepository.ObtenerPorIdAsync(casoId);

            if (caso is null)
                throw new NotFoundException("El caso no existe.");

            // Reglas de negocio
            if (caso.EstaCerrado())
                throw new BusinessConflictException("El caso ya está cerrado.");

            if (caso.Estado == EstadoCaso.EnProceso)
            {
                if (string.IsNullOrWhiteSpace(caso.Descripcion))
                    throw new InvalidEstadoCasoException(
                        "No se puede cerrar un caso sin descripción."
                    );

                caso.Estado = EstadoCaso.Cerrado;
                caso.FechaCierre = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(request.MotivoCierre))
                    caso.MotivoCierre = request.MotivoCierre;
            }
            else if (caso.Estado == EstadoCaso.Pendiente)
            {
                if (string.IsNullOrWhiteSpace(request.MotivoCierre))
                    throw new InvalidEstadoCasoException(
                        "Debe ingresar un motivo para cerrar un caso pendiente."
                    );

                caso.Estado = EstadoCaso.Cerrado;
                caso.FechaCierre = DateTime.UtcNow;
                caso.MotivoCierre = request.MotivoCierre;
            }
            else
            {
                throw new InvalidEstadoCasoException(
                    "No se puede cerrar este caso en su estado actual."
                );
            }
            // Auditoría y persistencia
            caso.UpdatedAt = DateTime.UtcNow;
            caso.ModifiedBy = userName;
            caso.FechaCambioEstado = DateTime.UtcNow;

            await _casoRepository.ActualizarAsync(caso);
        }
    }
}
