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
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        public RolesController(AppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerRoles()
        {
            var roles = await _context.Roles
                .Select(r => new { r.Id, r.Nombre })
                .ToListAsync();

            return Ok(roles);
        }
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerRolesAsignados(int usuarioId)
        {
            var roles = await _mediator.Send(new ObtenerRolesPorUsuarioQuery(usuarioId));
            return Ok(roles);
        }

        [HttpPost("{usuarioId}/{nombreRol}")]
        public async Task<IActionResult> AsignarRol(int usuarioId, string nombreRol)
        {
            var comando = new AsignarRolAUsuarioCommand
            {
                UsuarioId = usuarioId,
                NombreRol = nombreRol
            };

            await _mediator.Send(comando);
            return Ok(new { detail = "Rol asignado correctamente" });
        }

        [HttpDelete("{usuarioId}/{nombreRol}")]
        public async Task<IActionResult> QuitarRol(int usuarioId, string nombreRol)
        {
            await _mediator.Send(new QuitarRolAUsuarioCommand(usuarioId, nombreRol));
            return Ok();
        }



    }
}
