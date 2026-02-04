using Xunit;
using Moq;
using Aplicacion.Usuarios.Commands;
using Aplicacion.Repositorio;
using Aplicacion.Excepciones;
using Dominio.Entidades;

namespace Tests.Application.Usuarios
{
    public class EliminarUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private readonly Mock<ICasoRepository> _casoRepositoryMock;
        private readonly EliminarUsuarioCommandHandler _handler;

        public EliminarUsuarioCommandHandlerTests()
        {
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _casoRepositoryMock = new Mock<ICasoRepository>();

            _handler = new EliminarUsuarioCommandHandler(
                _usuarioRepositorioMock.Object,
                _casoRepositoryMock.Object
            );
        }
        [Fact]
        public async Task Handle_UsuarioIdInvalido_LanzaBusinessConflictException()
        {
            var usuarioId = 0;

            await Assert.ThrowsAsync<BusinessConflictException>(() =>
                _handler.Handle(new EliminarUsuarioCommand(usuarioId), CancellationToken.None)
            );

            _usuarioRepositorioMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
            _casoRepositoryMock.Verify(r => r.ExistenCasosCreadosPorUsuarioAsync(It.IsAny<string>()), Times.Never);
            _usuarioRepositorioMock.Verify(r => r.EliminarAsync(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositorioMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_UsuarioExisteYNoTieneCasos_EliminaUsuario()
        {
            // Arrange
            var usuarioId = 10;
            var usuario = new Usuario("Juan", "juan@test.com", "hash");

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            _casoRepositoryMock
                .Setup(r => r.ExistenCasosCreadosPorUsuarioAsync(usuario.Email))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(
                new EliminarUsuarioCommand(usuarioId),
                CancellationToken.None
            );

            // Assert
            _usuarioRepositorioMock.Verify(
                r => r.EliminarAsync(usuario),
                Times.Once
            );

            _usuarioRepositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_UsuarioNoExiste_LanzaNotFoundException()
        {
            // Arrange
            var usuarioId = 99;

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuarioId))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(
                    new EliminarUsuarioCommand(usuarioId),
                    CancellationToken.None
                )
            );

            _usuarioRepositorioMock.Verify(
                r => r.EliminarAsync(It.IsAny<Usuario>()),
                Times.Never
            );
            _usuarioRepositorioMock.Verify(
                r => r.GuardarCambiosAsync(),
                Times.Never
);
        }

        [Fact]
        public async Task Handle_UsuarioTieneCasos_LanzaBusinessConflictException()
        {
            // Arrange
            var usuarioId = 10;
            var usuario = new Usuario("Juan", "juan@test.com", "hash");

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            _casoRepositoryMock
                .Setup(r => r.ExistenCasosCreadosPorUsuarioAsync(usuario.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessConflictException>(() =>
                _handler.Handle(
                    new EliminarUsuarioCommand(usuarioId),
                    CancellationToken.None
                )
            );
            _usuarioRepositorioMock.Verify(
                r => r.EliminarAsync(It.IsAny<Usuario>()),
                Times.Never
            );
            _usuarioRepositorioMock.Verify(
            r => r.GuardarCambiosAsync(),
            Times.Never
);
        }
    }
}
