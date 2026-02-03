using Aplicacion.Usuarios.Commands;
using Aplicacion.Usuarios.Queries;
using Infraestructura.Persistencia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Soporte")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        //invocamos mediator para la contraseña 
        private readonly IMediator _mediator;
        public UsuariosController(AppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var resultado = await _mediator.Send(new ObtenerUsuariosQuery());
            return Ok(resultado);
        }
        [HttpGet("{usuarioId}/roles")]
        public async Task<IActionResult> ObtenerRolesAsignados(int usuarioId)
        {
            /*// 🧠 Se utiliza MediatR para enviar una Query que representa la solicitud de obtener roles.
            // Esto delega la responsabilidad al Handler correspondiente (en la capa de Aplicación).*/
            //throw new NotFoundException("Este usuario no existe en la base de datos (prueba).");
            var roles = await _mediator.Send(new ObtenerRolesPorUsuarioQuery(usuarioId));
            return Ok(roles);
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioCommand comando)
        {
            // Normalización (evita duplicados por mayúsculas/espacios)
            if (!string.IsNullOrWhiteSpace(comando.Email))
                comando.Email = comando.Email.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(comando.Nombre))
                comando.Nombre = comando.Nombre.Trim();

            var id = await _mediator.Send(comando);

            return CreatedAtAction(nameof(CrearUsuario), new { id }, new { id });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioRequest request)
        {
            var nombre = request.Nombre?.Trim();
            var email = request.Email?.Trim().ToLowerInvariant();
            var pass = request.Password;

            // Contrato (si quieres mantenerlo aquí además de ModelState)
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest(new ProblemDetails { Title = "Datos inválidos", Detail = "El nombre es obligatorio.", Status = 400, Instance = HttpContext.Request.Path });

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new ProblemDetails { Title = "Datos inválidos", Detail = "El email es obligatorio.", Status = 400, Instance = HttpContext.Request.Path });

            await _mediator.Send(new ActualizarUsuarioCommand(id, nombre, email, pass));
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            await _mediator.Send(new EliminarUsuarioCommand(id));
            return NoContent();
        }
    }

}
