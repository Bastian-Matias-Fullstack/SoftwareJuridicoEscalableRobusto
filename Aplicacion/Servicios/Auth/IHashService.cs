
namespace Aplicacion.Servicios.Auth
{
    public interface IHashService
    {
        string Hash(string textoPlano);
        bool Verificar(string textoPlano, string hashAlmacenado);
    }
}
