using Aplicacion.DTO;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Casos;
using Dominio.Entidades;
using Moq;
using Xunit;
using FluentAssertions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
public class CerrarCasoServiceTests
{
    private readonly Mock<ICasoRepository> _casoRepoMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CerrarCasoService _service;
    public CerrarCasoServiceTests()
    {
        _casoRepoMock = new Mock<ICasoRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _service = new CerrarCasoService(
            _casoRepoMock.Object,
            _httpContextAccessorMock.Object
        );
    }
    private void MockUser(string userName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext
        {
            User = principal
        };
        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);
    }

    [Fact]
    public async Task EjecutarAsync_RequestNull_LanzaInvalidEstadoCasoException()
    {
        // Arrange
        Func<Task> act = async () =>
            await _service.EjecutarAsync(1, null!);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidEstadoCasoException>();
    }

    [Fact]
    public async Task EjecutarAsync_CasoNoExiste_LanzaNotFoundException()
    {
        // Arrange
        var request = new CerrarCasoRequest
        {
            MotivoCierre = "Finalizado"
        };

        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new NotFoundException("No existe un caso con id 1."));

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }


    [Fact]
    public async Task EjecutarAsync_CasoYaCerrado_LanzaBusinessConflictException()
    {
        // Arrange
        var caso = new Caso
        {
            Estado = EstadoCaso.Cerrado
        };

        var request = new CerrarCasoRequest
        {
            MotivoCierre = "Duplicado"
        };

        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);

        // Act
        Func<Task> act = async () =>
            await _service.EjecutarAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<BusinessConflictException>();
    }

    [Fact]
    public async Task EjecutarAsync_EnProcesoSinDescripcion_LanzaInvalidEstadoCasoException()
    {
        // Arrange
        var caso = new Caso
        {
            Estado = EstadoCaso.EnProceso,
            Descripcion = ""
        };

        var request = new CerrarCasoRequest
        {
            MotivoCierre = "Finalizado"
        };

        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);

        // Act
        Func<Task> act = async () =>
            await _service.EjecutarAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidEstadoCasoException>();
    }

    [Fact]
    public async Task EjecutarAsync_PendienteSinMotivo_LanzaInvalidEstadoCasoException()
    {
        // Arrange
        var caso = new Caso
        {
            Estado = EstadoCaso.Pendiente
        };

        var request = new CerrarCasoRequest
        {
            MotivoCierre = ""
        };

        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);

        // Act
        Func<Task> act = async () =>
            await _service.EjecutarAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidEstadoCasoException>();
    }

    [Fact]
    public async Task EjecutarAsync_EnProcesoValido_CierraCasoCorrectamente()
    {
        // Arrange
        MockUser("usuario.test");
        var caso = new Caso
        {
            Estado = EstadoCaso.EnProceso,
            Descripcion = "Descripción válida"
        };

        var request = new CerrarCasoRequest
        {
            MotivoCierre = "Resuelto"
        };
        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);
        // Act
        await _service.EjecutarAsync(1, request);
        // Assert
        caso.Estado.Should().Be(EstadoCaso.Cerrado);
        caso.MotivoCierre.Should().Be("Resuelto");
        caso.ModifiedBy.Should().Be("usuario.test");
        caso.FechaCierre.Should().NotBeNull();
        _casoRepoMock.Verify(
            r => r.ActualizarAsync(It.IsAny<Caso>()),
            Times.Once
        );
    }

    [Fact]
    public async Task EjecutarAsync_PendienteValido_CierraCasoCorrectamente()
    {
        // Arrange
        MockUser("admin");
        var caso = new Caso
        {
            Estado = EstadoCaso.Pendiente
        };
        var request = new CerrarCasoRequest
        {
            MotivoCierre = "Cancelado"
        };
        _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(caso);
        // Act
        await _service.EjecutarAsync(1, request);
        // Assert
        caso.Estado.Should().Be(EstadoCaso.Cerrado);
        caso.MotivoCierre.Should().Be("Cancelado");
        caso.ModifiedBy.Should().Be("admin");

        _casoRepoMock.Verify(
            r => r.ActualizarAsync(It.IsAny<Caso>()),
            Times.Once
        );
    }
}
