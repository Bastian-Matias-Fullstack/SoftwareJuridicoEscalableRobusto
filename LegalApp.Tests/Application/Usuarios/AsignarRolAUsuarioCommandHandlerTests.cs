using Xunit;
using Moq;
using Aplicacion.Usuarios.Handlers;
using Aplicacion.Usuarios.Commands;
using Aplicacion.Repositorio;
using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Application.Usuarios
{
    public class AsignarRolAUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly Mock<IRolRepositorio> _rolRepositorioMock;
        private readonly AsignarRolAUsuarioCommandHandler _handler;

        public AsignarRolAUsuarioCommandHandlerTests()
        {
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _rolRepositorioMock = new Mock<IRolRepositorio>();

            _handler = new AsignarRolAUsuarioCommandHandler(
                _usuarioRepositorioMock.Object,
                _rolRepositorioMock.Object
            );
        }

        [Fact]
        public async Task Handle_UsuarioYRolValidos_AsignaRolCorrectamente()
        {
            // Arrange
            var usuario = new Usuario("Juan", "juan@test.com", "hash")
            {
                Id = 1,
                UsuarioRoles = new List<UsuarioRol>()
            };

            var rol = new Rol
            {
                Id = 2,
                Nombre = "Abogado"
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync(rol.Nombre))
                .ReturnsAsync(rol);

            var command = new AsignarRolAUsuarioCommand
            {
                UsuarioId = usuario.Id,
                NombreRol = "  " + rol.Nombre + "  "
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Single(usuario.UsuarioRoles);
            Assert.Contains(usuario.UsuarioRoles, ur => ur.RolId == rol.Id);

            _usuarioRepositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Usuario)null);

            var command = new AsignarRolAUsuarioCommand
            {
                UsuarioId = 99,
                NombreRol = "Admin"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None)

            );
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_RolNoExiste_LanzaExcepcion()
        {
            // Arrange
            var usuario = new Usuario("Juan", "juan@test.com", "hash")
            {
                Id = 1,
                UsuarioRoles = new List<UsuarioRol>()
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync(It.IsAny<string>()))
                .ReturnsAsync((Rol)null);

            var command = new AsignarRolAUsuarioCommand
            {
                UsuarioId = usuario.Id,
                NombreRol = "Admin"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_UsuarioYaTieneRol_LanzaExcepcion()
        {
            // Arrange
            var rol = new Rol
            {
                Id = 2,
                Nombre = "Admin"
            };

            var usuario = new Usuario("Juan", "juan@test.com", "hash")
            {
                Id = 1,
                UsuarioRoles = new List<UsuarioRol>
                {
                    new UsuarioRol { RolId = rol.Id }
                }
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            _rolRepositorioMock
                .Setup(r => r.ObtenerPorNombreAsync(rol.Nombre))
                .ReturnsAsync(rol);

            var command = new AsignarRolAUsuarioCommand
            {
                UsuarioId = usuario.Id,
                NombreRol = rol.Nombre
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _usuarioRepositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Never
            );
        }
    }
}
