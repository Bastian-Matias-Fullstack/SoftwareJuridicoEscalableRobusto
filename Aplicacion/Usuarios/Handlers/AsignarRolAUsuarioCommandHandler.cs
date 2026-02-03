using Aplicacion.Repositorio;
using Aplicacion.Usuarios.Commands;
using Dominio.Entidades;
using MediatR;

namespace Aplicacion.Usuarios.Handlers
{
    // Este handler responde al comando AsignarRolAUsuarioCommand
    // y no retorna datos (por eso usamos Unit como tipo de retorno).
    public class AsignarRolAUsuarioCommandHandler : IRequestHandler<AsignarRolAUsuarioCommand, Unit>
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IRolRepositorio _rolRepositorio;
        // Inyectamos los repositorios necesarios: Usuario y Rol
        public AsignarRolAUsuarioCommandHandler(IUsuarioRepositorio usuarioRepositorio, IRolRepositorio rolRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _rolRepositorio = rolRepositorio;
        }
        // Método principal que maneja el comando cuando se ejecuta Mediator.Send(...)
        public async Task<Unit> Handle(AsignarRolAUsuarioCommand request, CancellationToken cancellationToken)
        {
            // Paso 1: Validamos que el usuario exista
            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(request.UsuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            // Paso 2: Validamos que el rol exista
            var nombreRol = request.NombreRol.Trim();
            var rol = await _rolRepositorio.ObtenerPorNombreAsync(nombreRol);
            if (rol == null)
                throw new Exception("Rol no encontrado");

            // Paso 3: Validamos si el usuario ya tiene ese rol
            var yaTieneRol = usuario.UsuarioRoles.Any(ur => ur.RolId == rol.Id);
            if (yaTieneRol)
                throw new Exception("El usuario ya tiene este rol asignado");

            // Paso 4: Asignamos el nuevo rol al usuario
            usuario.UsuarioRoles.Add(new UsuarioRol
            {
                UsuarioId = usuario.Id,
                RolId = rol.Id
            });

            // Paso 5: Guardamos los cambios en la base de datos
            await _usuarioRepositorio.GuardarCambiosAsync();


            // Indicamos que la operación fue exitosa (sin retorno de datos)
            return Unit.Value;

        }
    }
}
