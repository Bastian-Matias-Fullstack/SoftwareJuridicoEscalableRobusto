using Aplicacion.DTO;
using Aplicacion.Repositorio;
using Aplicacion.Usuarios.DTO;
using Dominio.Entidades;
using Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repositorios
{
    // Implementación concreta de la interfaz IUsuarioRepositorio
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AppDbContext _context;
        // Acceso a base de datos EF Core a través de AppDbContext
        public UsuarioRepositorio(AppDbContext context)
        {
            _context = context;
        }
        // Verifica si existe un usuario con el email indicado
        public async Task<bool> ExistePorEmailAsync(string email, int excluirUsuarioId)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email && u.Id != excluirUsuarioId);
        }
        // Crea un nuevo usuario con sus roles y guarda los cambios en base de datos
        public async Task CrearUsuarioConRolesAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync(); // ⚠️ Se guarda directo aquí porque este método lo requiere
        }
        // Obtiene una lista de usuarios con sus roles, proyectando a UsuarioDto
        public async Task<List<UsuarioDto>> ObtenerUsuariosConRolesAsync(CancellationToken ct)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .AsNoTracking()
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Roles = u.UsuarioRoles
                        .Select(ur => ur.Rol.Nombre)
                        .ToList()
                })
                .ToListAsync(ct);
        }
        // Devuelve los roles asignados a un usuario específico (por su ID)
        public async Task<List<RolAsignadoDto>> ObtenerRolesAsignadosAsync(int usuarioId)
        {
            /*"Este método aplica una consulta LINQ sobre la tabla intermedia UsuarioRoles,
             * filtrando por usuarioId y usando Include para traer la entidad relacionada Rol. 
             * Luego proyecta los resultados en un DTO de solo lectura para mantener la separación
             * entre dominio y presentación. Todo se ejecuta de forma asincrónica, dentro de un patrón
             * repositorio que respeta la arquitectura limpia."*/
            var roles = await _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuarioId)
                .Include(ur => ur.Rol)
                .Select(ur => new RolAsignadoDto
                {
                    Nombre = ur.Rol.Nombre
                })
                .ToListAsync();
            return roles;
        }
        //Existe algún registro en la tabla Usuarios cuyo Id sea igual a X?
        public async Task<bool> ExistePorIdAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }
        // Obtiene un usuario completo por ID, incluyendo su lista de roles
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)  // ⚠️ Importante para poder agregar roles en el handler
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task EliminarAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
        }
        public async Task<bool> ExistePorEmailAsync(string email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email);
        }
        public async Task<bool> ExisteEmailAsync(string email, int usuarioIdExcluir)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email && u.Id != usuarioIdExcluir);
        }
        // Guarda todos los cambios pendientes en la base de datos (llamado desde handlers)
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
