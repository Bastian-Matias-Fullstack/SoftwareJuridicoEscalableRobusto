using Dominio.Entidades;

namespace Aplicacion.Repositorio
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObtenerPorNombreAsync(string nombre);
        Task CrearAsync(Cliente cliente);
        Task<Cliente?> ObtenerPorIdAsync(int id);
    }
}
