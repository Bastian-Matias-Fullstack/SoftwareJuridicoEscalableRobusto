using Aplicacion.Repositorio;
using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infraestructura.Persistencia;

namespace Infraestructura.Repositorios
{
    public class ClienteRepository : IClienteRepository
    {

        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Nombre == nombre);
        }

        public async Task CrearAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
    