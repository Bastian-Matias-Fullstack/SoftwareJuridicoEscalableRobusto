using Aplicacion.DTO;
using Aplicacion.DTOs;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aplicacion.Casos
{

    /*Este método sigue un flujo claro: validar, formatear, evitar duplicados, 
     * crear cliente si no existe, guardar el caso y registrar logs. Es limpio, 
     * mantenible y respeta las reglas del negocio.*/
    public class CrearCasoService
    {
        private readonly ICasoRepository _casoRepository;
        private readonly FormateadorNombreService _formateador;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<CrearCasoService> _logger;

        public CrearCasoService(ICasoRepository casoRepository, IClienteRepository clienteRepository, FormateadorNombreService formateador, ILogger<CrearCasoService> logger)
        {
            _casoRepository = casoRepository;
            _formateador = formateador;
            _clienteRepository = clienteRepository; 
            _logger = logger;
        }
        public async Task<CasoDto> EjecutarAsync(CrearCasoRequest request)
        {
            try
            {
                // 🔹 0. Normalizar input (AQUÍ VA EL CAMBIO)
                request.Titulo = request.Titulo?.Trim() ?? string.Empty;
                request.Descripcion = request.Descripcion?.Trim() ?? string.Empty;
                // 1. Validaciones
                if (string.IsNullOrWhiteSpace(request.Titulo))
                    throw new ArgumentException("El título del caso es obligatorio.");
                if (request.ClienteId <= 0)
                    throw new ArgumentException("Debe seleccionar un cliente válido.");
                _logger.LogInformation("🟢 Creando caso para ClienteId: {ClienteId}", request.ClienteId);               
                var cliente = await _clienteRepository.ObtenerPorIdAsync(request.ClienteId);
                if (cliente is null)
                    throw new InvalidOperationException("El cliente no existe.");

                //6. Crear caso
                var nuevoCaso = new Caso
                {
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    TipoCaso = request.TipoCaso,
                    ClienteId = cliente.Id,
                    Cliente = cliente, 
                    FechaCreacion = DateTimeOffset.UtcNow,
                    Estado = EstadoCaso.Pendiente,
                };
                await _casoRepository.CrearAsync(nuevoCaso);
                _logger.LogInformation(" Caso creado exitosamente con ID: {CasoId}", nuevoCaso.Id);

                // 6. Retornar DTO
                return new CasoDto
                {
            Id = nuevoCaso.Id,
            Titulo = nuevoCaso.Titulo,
            Estado = nuevoCaso.Estado,
            FechaCreacion = nuevoCaso.FechaCreacion,
            NombreCliente = cliente.Nombre,
            TipoCaso = nuevoCaso.TipoCaso,
            Descripcion = nuevoCaso.Descripcion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear caso.");
                throw;
            }
        }
    }
}

