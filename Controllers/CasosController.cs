using Aplicacion.Casos;
using Aplicacion.DTO;
using Aplicacion.DTOs;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Aplicacion.Servicios.Casos;
using Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Abogado")]
    public class CasosController : ControllerBase
    {
        private readonly ListarCasosService _listarCasosService;
        private readonly CrearCasoService _crearCasoService;
        private readonly CerrarCasoService _cerrarCasosService;
        private readonly ActualizarCasoService _actualizarCasoService;
        private readonly EliminarCasoService _eliminarCasoService;
        private readonly ICasoRepository _casoRepository;

        public CasosController(
            ListarCasosService listarCasosService,
            CrearCasoService crearCasosService,
            CerrarCasoService cerrarCasoService,
            ActualizarCasoService actualizarCasoService,
            EliminarCasoService eliminarCasoService,
            ICasoRepository casoRepository)
        {
            _listarCasosService = listarCasosService;
            _crearCasoService = crearCasosService;
            _actualizarCasoService = actualizarCasoService;
            _cerrarCasosService = cerrarCasoService;
            _eliminarCasoService = eliminarCasoService;
            _casoRepository = casoRepository;
        }

#if DEBUG
        [Authorize]
        // Endpoint solo para probar el middleware en desarrollo
        [HttpGet("error-test")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult LanzarError()
        {
            throw new Exception("🔥 Esto es una excepción de prueba");
        }
#endif
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerCasos([FromQuery] FiltroCasosRequest filtro)
        {
            if (filtro.Pagina < 1)
                filtro.Pagina = 1;

            if (filtro.Tamanio < 1)
                filtro.Tamanio = 10;

            // Normalización suave de strings
            filtro.Buscar = string.IsNullOrWhiteSpace(filtro.Buscar)
                ? null
                : filtro.Buscar.Trim();

            filtro.Estado = string.IsNullOrWhiteSpace(filtro.Estado)
                ? null
                : filtro.Estado.Trim();

            filtro.Orden = string.IsNullOrWhiteSpace(filtro.Orden)
                ? null
                : filtro.Orden.Trim();

            var resultado = await _listarCasosService.EjecutarAsync(filtro);

            if (resultado == null || resultado.Items == null || !resultado.Items.Any())
            {
                return Ok(new
                {
                    items = Array.Empty<object>(),
                    totalRegistros = 0,
                    pagina = filtro.Pagina,
                    tamanio = filtro.Tamanio,
                    totalPaginas = 0,
                    resumen = new { total = 0, pendientes = 0, resueltos = 0 }
                });
            }
            resultado.Items ??= new List<CasoDto>();

            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerCasoPorId(int id)
        {
            var caso = await _casoRepository.ObtenerPorIdAsync(id);

            if (caso == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Caso no encontrado",
                    Detail = $"No se encontró un caso con ID {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            var dto = new CasoDto
            {
                Id = caso.Id,
                Titulo = caso.Titulo,
                Estado = caso.Estado,
                FechaCreacion = caso.FechaCreacion,
                ClienteId = caso.ClienteId,
                NombreCliente = caso.Cliente?.Nombre,
                TipoCaso = caso.TipoCaso,
                Descripcion = caso.Descripcion,
                MotivoCierre = caso.MotivoCierre
            };

            return Ok(dto);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearCaso([FromBody] CrearCasoRequest request)
        {
            var nuevoCaso = await _crearCasoService.EjecutarAsync(request);
            return CreatedAtAction(nameof(ObtenerCasoPorId), new { id = nuevoCaso.Id }, nuevoCaso.Id);
        }

        [Authorize]
        [HttpPut("{id}/cerrar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CerrarCaso(
            int id,
            [FromBody] CerrarCasoRequest request)
        {
            await _cerrarCasosService.EjecutarAsync(id, request);
            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ActualizarCaso(
            int id,
            [FromBody] ActualizarCasoRequest request)
        {
            var esAdmin = User.IsInRole("Admin");
            await _actualizarCasoService.EjecutarAsync(id, request, esAdmin);
            return NoContent();
        }

        [Authorize]
        [HttpGet("conteo-casos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ConteoPorClienteDto>>> ObtenerConteoCasosPorCliente()
        {
            var resultado = await _casoRepository.ObtenerConteoCasosPorClienteAsync();
            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("estado/{estado}")]
        public async Task<IActionResult> GetPorEstado(string estado)
        {
            if (!Enum.TryParse<EstadoCaso>(estado, true, out var estadoEnum))
            {
                throw new InvalidEstadoCasoException(
                    $"El estado '{estado}' no es válido."
                );
            }

            var lista = await _casoRepository.ObtenerPorEstadoAsync(estadoEnum);

            return Ok(lista.Select(c => new CasoDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Estado = c.Estado,
                TipoCaso =c.TipoCaso,
                FechaCreacion = c.FechaCreacion,
                NombreCliente = c.NombreCliente
            }));
               
            }
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EliminarCaso(int id)
        {
            await _eliminarCasoService.EjecutarAsync(id);
            return NoContent();
        }

    }
}
