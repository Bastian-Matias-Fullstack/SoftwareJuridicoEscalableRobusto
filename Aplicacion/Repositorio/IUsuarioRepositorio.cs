using Aplicacion.DTO;
using Aplicacion.Usuarios.DTO;
using Dominio.Entidades;

namespace Aplicacion.Repositorio
{
    public interface IUsuarioRepositorio
    {
        Task<bool> ExistePorEmailAsync(string email, int excluirUsuarioId);
        Task CrearUsuarioConRolesAsync(Usuario usuario);
        Task<List<UsuarioDto>> ObtenerUsuariosConRolesAsync(CancellationToken ct);
        Task<List<RolAsignadoDto>> ObtenerRolesAsignadosAsync(int usuarioId);
        Task EliminarAsync(Usuario usuario);

        //“¿Existe un usuario con este Id en la base de datos?”
        Task<bool> ExistePorIdAsync(int id);

        // Obtiene un usuario con sus roles, o null si no existe
        Task<Usuario?> ObtenerPorIdAsync(int id);
        /// <summary>
        /// Verifica si existe un usuario con el email indicado.
        /// Usado en el caso de creación de usuarios.
        /// </summary>
        Task<bool> ExistePorEmailAsync(string email);

        /// <summary>
        /// Verifica si existe otro usuario con el mismo email,
        /// excluyendo al usuario actual.
        /// Usado en el caso de actualización de usuarios.
        /// </summary>
        Task<bool> ExisteEmailAsync(string email, int usuarioIdExcluir);
        Task GuardarCambiosAsync(); // ✅ NUEVO

    }
}
