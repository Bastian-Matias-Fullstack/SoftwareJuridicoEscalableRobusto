using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Soporte,Abogado")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new
                {
                    c.Id,
                    c.Nombre
                })
                .ToListAsync();

            return Ok(clientes);
        }
    }
}
