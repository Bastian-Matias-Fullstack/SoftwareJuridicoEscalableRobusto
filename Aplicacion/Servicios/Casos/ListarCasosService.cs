using Aplicacion.DTO;
using Aplicacion.DTOs;
using Aplicacion.Repositorio;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Servicios.Casos
{
    public class ListarCasosService
    {
        private readonly ICasoRepository _casoRepository;
        public ListarCasosService(ICasoRepository casoRepository)
        {
            _casoRepository = casoRepository;
        }
        public async Task<ResultadoPaginadoConResumen<CasoDto>> EjecutarAsync(FiltroCasosRequest filtro)
        {
            var query = _casoRepository.ObtenerQueryable();
            // Filtro por estado
            if (!string.IsNullOrWhiteSpace(filtro.Estado) && Enum.TryParse<EstadoCaso>(filtro.Estado, true, out var estadoEnum))
            {
                query = query.Where(c => c.Estado == estadoEnum);
            }
            // Búsqueda por texto libre
            if (!string.IsNullOrWhiteSpace(filtro.Buscar))
            {
                query = query.Where(c =>
                    c.Titulo.Contains(filtro.Buscar) ||
                    c.Cliente.Nombre.Contains(filtro.Buscar));
            }
            // Ordenamiento dinámico
            query = filtro.Orden switch
            {
                "fecha_desc" => query.OrderByDescending(c => c.FechaCreacion),
                "fecha_asc" => query.OrderBy(c => c.FechaCreacion),
                "titulo_asc" => query.OrderBy(c => c.Titulo),
                "titulo_desc" => query.OrderByDescending(c => c.Titulo),
                _ => query.OrderByDescending(c => c.Id)
            };
            // Total sin paginar
            var total = await query.CountAsync();
            // Paginación
            var skip = (filtro.Pagina - 1) * filtro.Tamanio;
            var items = await query
                .Skip(skip)
                .Take(filtro.Tamanio)
                .Select(c => new CasoDto
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Estado = c.Estado,
                    FechaCreacion = c.FechaCreacion,
                    ClienteId = c.ClienteId,          
                    NombreCliente = c.Cliente.Nombre,
                    Descripcion = c.Descripcion ?? "",
                    MotivoCierre = c.MotivoCierre ?? "",
                    TipoCaso = c.TipoCaso
                })
                .ToListAsync();
            var resumen = new ResumenCasosDto
            {
                Total = total,
                Pendientes = await query.CountAsync(c => c.Estado == EstadoCaso.Pendiente),
                Resueltos = await query.CountAsync(c => c.Estado == EstadoCaso.Cerrado)
            };
            return new ResultadoPaginadoConResumen<CasoDto>
            {
                Items = items,
                TotalRegistros = total,
                Pagina = filtro.Pagina,
                Tamanio = filtro.Tamanio,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.Tamanio), // ✅ Línea nueva
                Resumen = resumen
            };
        }
    }
}
