using Aplicacion.DTO;
using Dominio.Entidades;

namespace Aplicacion.Repositorio
{
    public interface ICasoRepository
    {
        Task CrearAsync(Caso nuevoCaso);
        Task<Caso?> ObtenerPorIdAsync(int casoId);
        Task<IEnumerable<Caso>> ObtenerTodosAsync();
        Task ActualizarAsync(Caso caso);
        Task<Caso?> ObtenerPorId(int id);
        Task EliminarAsync(Caso caso);
        Task<List<ConteoPorClienteDto>> ObtenerConteoCasosPorClienteAsync();
        IQueryable<Caso> ObtenerQueryable();
        Task<List<Caso>> ObtenerPorEstadoAsync(EstadoCaso estado);
        Task<bool> ExistenCasosCreadosPorUsuarioAsync(string email);
        Task<bool> ExisteCasoActivoParaClienteAsync(int clienteId, int casoId);

    }
}


