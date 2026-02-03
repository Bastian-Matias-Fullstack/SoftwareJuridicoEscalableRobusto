using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infraestructura.Persistencia; // Asegúrate de tener esto

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context; // Paso 1

        public AdminController(AppDbContext context) // Paso 2
        {
            _context = context;
        }

        // 🔐 Solo usuarios con rol "Admin" pueden acceder
        [Authorize(Roles = "Admin")]
        [HttpGet("solo-admins")]
        public IActionResult SoloAdmins()
        {
            var username = User.Identity?.Name;
            return Ok($"🎉 Bienvenido {username}, accediste como Admin.");
        }

    }
}
    
