using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Auth;
using Aplicacion.Usuarios.Commands;
using Aplicacion.Usuarios.Handlers;
using Dominio.Entidades;
using Moq;
using System.Reflection.Metadata;
using Xunit;

namespace Tests.Application.Usuarios
{
    public class CrearUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly CrearUsuarioCommandHandler _handler;

        public CrearUsuarioCommandHandlerTests()
        {
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _hashServiceMock = new Mock<IHashService>();

            _handler = new CrearUsuarioCommandHandler(
                _usuarioRepositorioMock.Object,
                _hashServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_EmailNoExiste_CreaUsuarioCorrectamente()
        {
            // Arrange
            var command = new CrearUsuarioCommand
            {
                Nombre = "Juan Pérez",
                Email = "juan@test.com",
                Password = "123456"
            };

            _usuarioRepositorioMock
                .Setup(r => r.ExistePorEmailAsync(command.Email))
                .ReturnsAsync(false);

            _hashServiceMock
                .Setup(h => h.Hash(command.Password))
                .Returns("hashed-password");

            _usuarioRepositorioMock
                .Setup(r => r.CrearUsuarioConRolesAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => u.Id = 123)   // 👈 AQUÍ (simula DB)
                .Returns(Task.CompletedTask);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(123, result);
            _usuarioRepositorioMock.Verify(
                r => r.CrearUsuarioConRolesAsync(It.Is<Usuario>(
                    u => u.Email == command.Email &&
                         u.Nombre == command.Nombre &&
                         u.PasswordHash == "hashed-password"
                )),
                Times.Once
            );
        }
    [Fact]
    public async Task Handle_EmailExiste_LanzaExcepcion()
    {
        // Arrange
        var command = new CrearUsuarioCommand
        {
            Nombre = "Juan Pérez",
            Email = "juan@test.com",
            Password = "123456"
        };

        _usuarioRepositorioMock
            .Setup(r => r.ExistePorEmailAsync(command.Email))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<BusinessConflictException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        // Assert
        Assert.Equal("El correo ya está registrado.", ex.Message);

        _usuarioRepositorioMock.Verify(
            r => r.CrearUsuarioConRolesAsync(It.IsAny<Usuario>()),
            Times.Never
        );
        _hashServiceMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }
}

}
