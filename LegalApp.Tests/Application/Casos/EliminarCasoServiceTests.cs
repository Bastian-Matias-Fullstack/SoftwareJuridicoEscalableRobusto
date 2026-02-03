using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Moq;
using Xunit;
using FluentAssertions;
public class EliminarCasoServiceTests
{
    private readonly Mock<ICasoRepository> _casoRepoMock;
    private readonly EliminarCasoService _service;
    public EliminarCasoServiceTests()
    {
        _casoRepoMock = new Mock<ICasoRepository>();

        _service = new EliminarCasoService(
            _casoRepoMock.Object
        );
    }
    [Fact]
    public async Task EjecutarAsync_CasoNoExiste_LanzaNotFoundException()
    {
        // Arrange
        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Caso)null!);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(99);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>();

        _casoRepoMock.Verify(
            r => r.EliminarAsync(It.IsAny<Caso>()),
            Times.Never
        );
    }
    [Fact]
    public async Task EjecutarAsync_CasoCerrado_LanzaBusinessConflictException()
    {
        // Arrange
        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.Cerrado
        };

        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessConflictException>();

        _casoRepoMock.Verify(
            r => r.EliminarAsync(It.IsAny<Caso>()),
            Times.Never
        );
    }
    [Fact]
    public async Task EjecutarAsync_CasoValido_EliminaCasoCorrectamente()
    {
        // Arrange
        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.Pendiente
        };
        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);

        // Act
        await _service.EjecutarAsync(1);

        // Assert
        _casoRepoMock.Verify(
            r => r.EliminarAsync(caso),
            Times.Once
        );
    }
}

