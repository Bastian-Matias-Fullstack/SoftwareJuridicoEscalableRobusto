using MediatR;

namespace Aplicacion.Usuarios.Commands
{
    // IRequest<Unit> Indica que es una acción que no retorna datos, solo confirma éxito o error
    public class QuitarRolAUsuarioCommand : IRequest<Unit>
    {
        //Identifica al usuario al que se le quiere quitar un rol
        public int UsuarioId { get; set; }
        //El nombre del rol que se desea eliminar
        public string NombreRol { get; set; } = string.Empty;
        public QuitarRolAUsuarioCommand(int usuarioId, string nombreRol)
        {
            UsuarioId = usuarioId;
            NombreRol = nombreRol;
        }
    }
}
