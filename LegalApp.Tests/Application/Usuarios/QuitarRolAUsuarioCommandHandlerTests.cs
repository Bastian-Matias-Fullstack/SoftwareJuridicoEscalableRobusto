using Xunit;
using Moq;
using Aplicacion.Usuarios.Handlers;
using Aplicacion.Usuarios.Commands;
using Aplicacion.Repositorio;
using Dominio.Entidades;

namespace Tests.Application.Usuarios
{
    public class QuitarRolAUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly Mock<IRolRepositorio> _rolRepositorioMock;
        private readonly QuitarRolAUsuarioCommandHandler _handler;

        public QuitarRolAUsuarioCommandHandlerTests()
        {
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _rolRepositorioMock = new Mock<IRolRepositorio>();

            _handler = new QuitarRolAUsuarioCommandHandler(
                _usuarioRepositorioMock.Object,
                _rolRepositorioMock.Object
            );
        }
        //Regla A — No se puede quitar Admin al usuario principal (Id = 1)
        [Fact]
        public async Task Handle_QuitarAdminAUsuarioPrincipal_LanzaInvalidOperationException()
        {
            var command = new QuitarRolAUsuarioCommand(1, "Admin");

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }
        //Regla B — Rol no existe
        [Fact]
        public async Task Handle_RolNoExiste_LanzaKeyNotFoundException()
        {
            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync(It.IsAny<string>()))
                .ReturnsAsync((Rol?)null);

            var command = new QuitarRolAUsuarioCommand(2, "Admin");

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
            _usuarioRepositorioMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
            //Aquí debe ser Once, porque sí se consulta el rol
            _rolRepositorioMock.Verify(r => r.ObtenerPorNombreAsync(It.IsAny<string>()), Times.Once);
        }
        //Regla C — Usuario no existe
        [Fact]
        public async Task Handle_UsuarioNoExiste_LanzaKeyNotFoundException()
        {
            var rol = new Rol { Id = 1, Nombre = "Admin" };

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync("Admin"))
                .ReturnsAsync(rol);

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Usuario?)null);

            var command = new QuitarRolAUsuarioCommand(99, "Admin");

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }

        //Regla D — Usuario no tiene el rol
        [Fact]
        public async Task Handle_UsuarioNoTieneRol_LanzaInvalidOperationException()
        {
            var rol = new Rol { Id = 1, Nombre = "Admin" };

            var usuario = new Usuario("Juan", "juan@test.com", "hash")
            {
                Id = 2,
                UsuarioRoles = new List<UsuarioRol>() // sin roles
            };

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync("Admin"))
                .ReturnsAsync(rol);

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            var command = new QuitarRolAUsuarioCommand(usuario.Id, "Admin");

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }

        //Regla E — Caso válido: se quita el rol correctamente
        [Fact]
        public async Task Handle_RolValido_SeQuitaCorrectamente()
        {
            var rol = new Rol { Id = 1, Nombre = "Admin" };

            var usuario = new Usuario("Juan", "juan@test.com", "hash")
            {
                Id = 2,
                UsuarioRoles = new List<UsuarioRol>
                {
                    new UsuarioRol { RolId = rol.Id }
                }
            };

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync("Admin"))
                .ReturnsAsync(rol);

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            var command = new QuitarRolAUsuarioCommand(usuario.Id, "  Admin  ");
            await _handler.Handle(command, CancellationToken.None);

            Assert.Empty(usuario.UsuarioRoles);

            _usuarioRepositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Once
            );
        }
    }
}
