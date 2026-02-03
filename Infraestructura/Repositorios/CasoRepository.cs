using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Infraestructura.Persistencia;
using Aplicacion.Repositorio;
using Aplicacion.DTO;

namespace Infraestructura.Repositorios
{
    public class CasoRepository : ICasoRepository
    {
        private readonly AppDbContext _context;
        public CasoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Caso>> ObtenerTodosAsync()
        {
            return await _context.Casos.ToListAsync();
        }
        public async Task<Caso?> ObtenerPorIdAsync(int casoId)
        {
            return await _context.Casos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.Id == casoId);
        }
        public async Task CrearAsync(Caso nuevoCaso)
        {
            _context.Casos.Add(nuevoCaso);
            await _context.SaveChangesAsync();
        }
        public async Task ActualizarAsync(Caso caso)
        {
            _context.Casos.Update(caso);
            await _context.SaveChangesAsync();
        }
        public async Task EliminarAsync(Caso caso)
        {
            _context.Casos.Remove(caso);
            await _context.SaveChangesAsync();
        }
        public async Task<Caso?> ObtenerPorId(int id)
        {
            return await _context.Casos.FindAsync(id);
        }
        public async Task<List<ConteoPorClienteDto>> ObtenerConteoCasosPorClienteAsync()
        {
            return await _context.Casos
             .Include(c => c.Cliente) // 👈 esto trae el nombre del cliente
             .GroupBy(c => new { c.ClienteId, c.Cliente.Nombre })
             .Select(g => new ConteoPorClienteDto
             {
                 ClienteId = g.Key.ClienteId,
                 NombreCliente = g.Key.Nombre,
                 CantidadCasos = g.Count()
             })
             .ToListAsync();
        }
        public IQueryable<Caso> ObtenerQueryable()
        {
            return _context.Casos
                .Include(c => c.Cliente)
                .AsNoTracking(); // Mejora rendimiento para lectura
        }
        public async Task<List<Caso>> ObtenerPorEstadoAsync(EstadoCaso estado)
        {
            return await _context.Casos
                .Where(c => c.Estado == estado)
                .ToListAsync();
        }
        public async Task<bool> ExistenCasosCreadosPorUsuarioAsync(string email)
        {
            return await _context.Casos
                .AnyAsync(c => c.CreatedBy == email);
        }
        public async Task<bool> ExisteCasoActivoParaClienteAsync(int clienteId, int casoId)
        {
            return await _context.Casos.AnyAsync(c =>
        c.ClienteId == clienteId &&
        c.Id != casoId &&
        c.Estado != EstadoCaso.Cerrado
    );
        }

    }
}
