using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Auth;
using Aplicacion.Usuarios.Commands;
using Aplicacion.Usuarios.Handlers;
using Dominio.Entidades;
using Moq;
using Xunit;

namespace Aplicacion.Tests.Usuarios
{
    public class ActualizarUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepositorio> _repositorioMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly ActualizarUsuarioCommandHandler _handler;

        public ActualizarUsuarioCommandHandlerTests()
        {
            _repositorioMock = new Mock<IUsuarioRepositorio>();
            _hashServiceMock = new Mock<IHashService>();

            _handler = new ActualizarUsuarioCommandHandler(
                _repositorioMock.Object,
                _hashServiceMock.Object
            );
        }

        // -------------------------------------------------------
        // 1️⃣ HAPPY PATH
        // -------------------------------------------------------
        [Fact]
        public async Task Handle_DeberiaActualizarUsuario_CuandoDatosSonValidos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Nombre Antiguo",
                Email = "antiguo@test.com",
                PasswordHash = "hash-antiguo"
            };

            _repositorioMock
                .Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _repositorioMock
                .Setup(r => r.ExisteEmailAsync("nuevo@test.com", 1))
                .ReturnsAsync(false);

            _hashServiceMock
                .Setup(h => h.Hash("123456"))
                .Returns("hash-nuevo");

            var command = new ActualizarUsuarioCommand(
                1,
                "Nombre Nuevo",
                "nuevo@test.com",
                "123456"
            );

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Nombre Nuevo", usuario.Nombre);
            Assert.Equal("nuevo@test.com", usuario.Email);
            Assert.Equal("hash-nuevo", usuario.PasswordHash);

            _repositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Once
            );
        }

        // -------------------------------------------------------
        // 2️⃣ USUARIO NO EXISTE
        // -------------------------------------------------------
        [Fact]
        public async Task Handle_DeberiaLanzarNotFoundException_SiUsuarioNoExiste()
        {
            // Arrange
            _repositorioMock
                .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Usuario)null);

            var command = new ActualizarUsuarioCommand(
                99,
                "Nombre",
                "email@test.com",
                null
            );

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        // -------------------------------------------------------
        // 3️⃣ EMAIL DUPLICADO
        // -------------------------------------------------------
        [Fact]
        public async Task Handle_DeberiaLanzarBusinessConflictException_SiEmailYaExiste()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "original@test.com"
            };

            _repositorioMock
                .Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _repositorioMock
                .Setup(r => r.ExisteEmailAsync("duplicado@test.com", 1))
                .ReturnsAsync(true);

            var command = new ActualizarUsuarioCommand(
                1,
                "Nombre",
                "duplicado@test.com",
                null
            );

            // Act & Assert
            await Assert.ThrowsAsync<BusinessConflictException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }

        // -------------------------------------------------------
        // 4️⃣ PASSWORD NULL → NO REHASHEAR
        // -------------------------------------------------------
        [Fact]
        public async Task Handle_NoDeberiaCambiarPassword_SiPasswordEsNull()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                PasswordHash = "hash-original"
            };

            _repositorioMock
                .Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _repositorioMock
                .Setup(r => r.ExisteEmailAsync(It.IsAny<string>(), 1))
                .ReturnsAsync(false);

            var command = new ActualizarUsuarioCommand(
                1,
                "Nombre",
                "email@test.com",
                null
            );
            // Act
            await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.Equal("hash-original", usuario.PasswordHash);

            _hashServiceMock.Verify(
                h => h.Hash(It.IsAny<string>()),
                Times.Never
            );
        }
        [Fact]
        public async Task Handle_NoDeberiaCambiarPassword_SiPasswordEsWhitespace()
        {
            var usuario = new Usuario { Id = 1, PasswordHash = "hash-original" };

            _repositorioMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
            _repositorioMock.Setup(r => r.ExisteEmailAsync(It.IsAny<string>(), 1)).ReturnsAsync(false);

            var command = new ActualizarUsuarioCommand(1, "Nombre", "email@test.com", "   ");

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("hash-original", usuario.PasswordHash);
            _hashServiceMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
            _repositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Once);
        }

    }
}
