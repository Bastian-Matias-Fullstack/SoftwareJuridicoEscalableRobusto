namespace Aplicacion.Servicios.Auth
{
    public interface IJwtService
    {
        string GenerarToken(string email, int usuarioId, List<string> roles);
    }
}
