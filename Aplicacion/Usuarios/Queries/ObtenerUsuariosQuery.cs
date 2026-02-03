using Aplicacion.Usuarios.DTO;
using MediatR;

namespace Aplicacion.Usuarios.Queries
{
    public class ObtenerUsuariosQuery : IRequest<List<UsuarioDto>>
    {

    }
}
