// Importamos la entidad del dominio que vamos a consultar (Rol)
using Dominio.Entidades;

// Importamos el contexto de EF Core que nos conecta con la base de datos
using Infraestructura.Persistencia;

// Importamos herramientas de EF Core para ejecutar consultas
using Microsoft.EntityFrameworkCore;

// Importamos la interfaz que esta clase implementará (principio de inversión de dependencias)
using Aplicacion.Repositorio;

// Espacio de nombres correspondiente al proyecto de Infraestructura
namespace Infraestructura.Repositorios
{
    // Esta clase implementa el contrato IRolRepositorio y debe ser pública para poder ser inyectada (DI)
    public class RolRepositorio : IRolRepositorio
    {
        // Campo privado que almacena la instancia del DbContext
        private readonly AppDbContext _context;
        // Constructor: recibe el contexto vía inyección de dependencias
        public RolRepositorio(AppDbContext context)
        {
            _context = context;
        }
        // Método asincrónico que busca un rol por su nombre exacto
        public async Task<Rol?> ObtenerPorNombreAsync(string nombre)
        {
            // Consulta con EF Core que busca el primer rol cuyo nombre coincida exactamente
            // Si no encuentra ninguno, devuelve null (por eso es Rol?)
            return await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre);
        }
    }
}
