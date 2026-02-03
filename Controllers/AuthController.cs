using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aplicacion.Servicios.Auth;
using Aplicacion.DTO;
using Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _context;
    private readonly IHashService _hashService;

    public AuthController(AppDbContext context,  IJwtService jwtService, IHashService hashService)
    {
        _jwtService = jwtService;
        _context = context;
        _hashService = hashService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            /* aqui usamos EF CORE YA QUE ES CONSULTA SIMPLE */
            var usuario = await _context.Usuarios
           .Include(u => u.UsuarioRoles)
           .ThenInclude(ur => ur.Rol)
           .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            // Verificar contraseña con IHashService
            var esValida = _hashService.Verificar(dto.Password, usuario.PasswordHash);
            if (!esValida)
                return Unauthorized("Contraseña incorrecta");

            // Extraer los nombres de los roles
            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

            // Generar token JWT
            var token = _jwtService.GenerarToken(usuario.Email, usuario.Id, roles);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
}
