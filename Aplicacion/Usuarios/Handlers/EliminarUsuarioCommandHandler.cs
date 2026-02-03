using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using MediatR;

namespace Aplicacion.Usuarios.Commands
{
    public class EliminarUsuarioCommandHandler : IRequestHandler<EliminarUsuarioCommand, Unit>
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly ICasoRepository _casoRepository;
        public EliminarUsuarioCommandHandler(IUsuarioRepositorio repositorio, ICasoRepository casoRepository)
        {
            _repositorio = repositorio;
            _casoRepository = casoRepository;
        }
        public async Task<Unit> Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (request.UsuarioId <= 0)
                throw new BusinessConflictException("Id de usuario inválido.");
                    var usuario = await _repositorio.ObtenerPorIdAsync(request.UsuarioId);
            if (usuario is null)
                throw new NotFoundException(
                    $"Usuario con id {request.UsuarioId} no existe.");
            var tieneCasos = await _casoRepository
                .ExistenCasosCreadosPorUsuarioAsync(usuario.Email);
            if (tieneCasos)
                throw new BusinessConflictException(
                    "No se puede eliminar el usuario porque tiene casos asociados.");
            await _repositorio.EliminarAsync(usuario);
            await _repositorio.GuardarCambiosAsync();
            return Unit.Value;
        }

    }
}
