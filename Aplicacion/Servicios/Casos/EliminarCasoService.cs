using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Dominio.Entidades;

namespace Aplicacion.Servicios
{
    public class EliminarCasoService
    {
        private readonly ICasoRepository _repositorio;

        public EliminarCasoService(ICasoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task EjecutarAsync(int id)
        {
            // 1. Buscar el caso
            var caso = await _repositorio.ObtenerPorIdAsync(id);

            // 2. Regla de dominio: el caso debe existir
            if (caso == null)
                throw new NotFoundException($"No existe un caso con id {id}.");

            // 3. Regla de negocio: no se puede eliminar un caso cerrado
            if (caso.Estado == EstadoCaso.Cerrado)
                throw new BusinessConflictException("No se puede eliminar un caso cerrado.");

            // 4. Eliminación válida
            await _repositorio.EliminarAsync(caso);
        }
    }
}
