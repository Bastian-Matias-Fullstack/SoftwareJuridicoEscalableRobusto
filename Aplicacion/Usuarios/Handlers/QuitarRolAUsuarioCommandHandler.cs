using Aplicacion.Repositorio;
using Aplicacion.Usuarios.Commands;
using MediatR;

namespace Aplicacion.Usuarios.Handlers
{
    public class QuitarRolAUsuarioCommandHandler : IRequestHandler<QuitarRolAUsuarioCommand, Unit>
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IRolRepositorio _rolRepositorio;
        public QuitarRolAUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio, IRolRepositorio rolRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _rolRepositorio = rolRepositorio;
        }
        public async Task<Unit> Handle(QuitarRolAUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (request.UsuarioId == 1 && request.NombreRol.Trim().ToLower() == "admin")
            {
                throw new InvalidOperationException("No se puede quitar el rol Admin del usuario principal.");
            }
            var rol = await _rolRepositorio.ObtenerPorNombreAsync(request.NombreRol.Trim());
            if (rol == null)
            {
                throw new KeyNotFoundException("Rol no encontrado.");
            }
            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(request.UsuarioId);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }
            var usuarioRol = usuario.UsuarioRoles.FirstOrDefault(ur => ur.RolId == rol.Id);
            if (usuarioRol == null)
            {
                throw new InvalidOperationException("El usuario no tiene ese rol asignado.");
            }
            usuario.UsuarioRoles.Remove(usuarioRol);
            await _usuarioRepositorio.GuardarCambiosAsync();
            return Unit.Value;
        }
    }
}
