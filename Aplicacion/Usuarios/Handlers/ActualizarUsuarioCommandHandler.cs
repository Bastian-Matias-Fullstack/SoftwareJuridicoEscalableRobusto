using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Auth;
using Aplicacion.Usuarios.Commands;
using MediatR;

namespace Aplicacion.Usuarios.Handlers
{
    public class ActualizarUsuarioCommandHandler : IRequestHandler<ActualizarUsuarioCommand, Unit>
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IHashService _hashService;
        public ActualizarUsuarioCommandHandler(IUsuarioRepositorio repositorio, IHashService hashService)
        {
            _repositorio = repositorio;
            _hashService = hashService;
        }
        public async Task<Unit> Handle(
                    ActualizarUsuarioCommand request,
                    CancellationToken cancellationToken)
        {
            // 1️⃣ Obtener usuario
            var usuario = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado");

            // 2️⃣ Validar email duplicado (ANTES de guardar)
            var emailExiste = await _repositorio.ExisteEmailAsync(
                request.Email,
                request.Id // excluir el mismo usuario
            );

            if (emailExiste)
                throw new BusinessConflictException(
                    "El correo electrónico ya está registrado."
                );
            // 3️⃣ Aplicar cambios
            usuario.Nombre = request.Nombre;
            usuario.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                usuario.PasswordHash = _hashService.Hash(request.Password);
            }
            // 4️⃣ Persistir
            await _repositorio.GuardarCambiosAsync();
            return Unit.Value;
        }
    }
}
