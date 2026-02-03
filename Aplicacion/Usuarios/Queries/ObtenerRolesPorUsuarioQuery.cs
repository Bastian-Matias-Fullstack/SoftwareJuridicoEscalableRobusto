using Aplicacion.DTO;
using MediatR;

namespace Aplicacion.Usuarios.Queries
{
        public record ObtenerRolesPorUsuarioQuery(int UsuarioId) : IRequest<List<RolAsignadoDto>>;
}
