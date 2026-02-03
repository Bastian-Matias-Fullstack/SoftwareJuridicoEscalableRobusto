using Aplicacion.DTO;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Usuarios.Queries;
using MediatR;

namespace Aplicacion.Usuarios.Handlers
{
    public class ObtenerRolesPorUsuarioQueryHandler : IRequestHandler <ObtenerRolesPorUsuarioQuery, List<RolAsignadoDto>>
    {
        private readonly IUsuarioRepositorio _repositorio;
        public ObtenerRolesPorUsuarioQueryHandler (IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<List<RolAsignadoDto>> Handle(ObtenerRolesPorUsuarioQuery request , CancellationToken cancellationToken)
        {
            var existeUsuario = await _repositorio.ExistePorIdAsync(request.UsuarioId);
            if (!existeUsuario)
                throw new NotFoundException("El usuario solicitado no existe en la base de datos.");

            var roles = await _repositorio.ObtenerRolesAsignadosAsync(request.UsuarioId);

            if (roles == null || roles.Count == 0)
                throw new NotFoundException("El usuario no tiene roles asignados.");
            return roles;
        }
    }
}
