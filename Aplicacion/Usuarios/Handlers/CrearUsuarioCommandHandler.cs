using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Auth;
using Aplicacion.Usuarios.Commands;
using Dominio.Entidades;
using MediatR;


namespace Aplicacion.Usuarios.Handlers
{
    // Este handler procesa el comando CrearUsuarioCommand y devuelve un int (el Id del nuevo usuario)
    public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, int>
    {
        // Inyectamos el repositorio que nos permite acceder a la base de datos
        private readonly IUsuarioRepositorio _repositorio;

        // Inyectamos el servicio que nos permite hashear la contraseña
        private readonly IHashService _hashService;
        // Constructor: el framework inyectará las dependencias configuradas en Program.cs
        public CrearUsuarioCommandHandler(IUsuarioRepositorio repositorio, IHashService hashService)
        {
            _repositorio = repositorio;
            _hashService = hashService;
        }
        public async Task<int> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
        {
            var existe = await _repositorio.ExistePorEmailAsync(request.Email);
            if (existe)
                throw new BusinessConflictException("El correo ya está registrado.");
            var passwordHash = _hashService.Hash(request.Password);

            var nuevoUsuario = new Usuario(
                nombre: request.Nombre,
                email: request.Email,
                passwordHash: passwordHash
            );

            await _repositorio.CrearUsuarioConRolesAsync(nuevoUsuario);
            return nuevoUsuario.Id;
        }

    }
}
