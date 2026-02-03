using Dominio.Entidades;

namespace Aplicacion.Repositorio
{
    public interface IRolRepositorio
    {
        Task<Rol?> ObtenerPorNombreAsync(string nombre);
    }
}
