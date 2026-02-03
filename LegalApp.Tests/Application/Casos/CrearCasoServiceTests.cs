using Aplicacion.Casos;
using Aplicacion.DTO;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Dominio.Entidades;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
public class CrearCasoServiceTests
{
    private readonly Mock<ICasoRepository> _casoRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<ILogger<CrearCasoService>> _loggerMock;
    private readonly FormateadorNombreService _formateador;
    private readonly CrearCasoService _service;
    public CrearCasoServiceTests()
    {
        _casoRepoMock = new Mock<ICasoRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _loggerMock = new Mock<ILogger<CrearCasoService>>();
        _formateador = new FormateadorNombreService();
        _service = new CrearCasoService(
            _casoRepoMock.Object,
            _clienteRepoMock.Object,
            _formateador,
            _loggerMock.Object
        );
    }
    [Fact]
    public async Task EjecutarAsync_CasoValido_CreaCasoYRetornaDto()
    {
        // Arrange
        var cliente = new Cliente
        {
            Id = 1,
            Nombre = "Juan Perez"
        };

        var request = new CrearCasoRequest
        {
            Titulo = "Caso de prueba",
            Descripcion = "Descripción",
            ClienteId = cliente.Id,      // ✅ CAMBIO
            TipoCaso = TipoCaso.Civil
        };
    
        _clienteRepoMock
            .Setup(r => r.ObtenerPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);
        // Act
        var result = await _service.EjecutarAsync(request);
        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be(request.Titulo);
        result.Estado.Should().Be(EstadoCaso.Pendiente);
        result.NombreCliente.Should().Be(cliente.Nombre);
        _casoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Caso>()), Times.Once);
    }
    [Fact]
    public async Task EjecutarAsync_TituloVacio_LanzaArgumentException()
    {
        // Arrange
        var request = new CrearCasoRequest
        {
            Titulo = "   ",
            Descripcion = "Desc",
            ClienteId = 1,
            TipoCaso = TipoCaso.Civil
        };

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();

        _clienteRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        _casoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Caso>()), Times.Never);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task EjecutarAsync_ClienteIdInvalido_LanzaArgumentException(int clienteId)
    {
        // Arrange
        var request = new CrearCasoRequest
        {
            Titulo = "Caso válido",
            Descripcion = "Desc",
            ClienteId = clienteId,
            TipoCaso = TipoCaso.Civil
        };

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();

        _clienteRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        _casoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Caso>()), Times.Never);
    }
    [Fact]
    public async Task EjecutarAsync_ClienteNoExiste_LanzaInvalidOperationException()
    {
        // Arrange
        var request = new CrearCasoRequest
        {
            Titulo = "Caso válido",
            Descripcion = "Desc",
            ClienteId = 1,
            TipoCaso = TipoCaso.Civil
        };

        _clienteRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync((Cliente?)null);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        _casoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Caso>()), Times.Never);
    }
    [Fact]
    public async Task EjecutarAsync_TituloYDescripcionConEspacios_SeNormalizanAntesDeCrear()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nombre = "Juan Perez" };

        var request = new CrearCasoRequest
        {
            Titulo = "   Caso con espacios   ",
            Descripcion = "   Descripción con espacios   ",
            ClienteId = 1,
            TipoCaso = TipoCaso.Civil
        };

        _clienteRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(cliente);

        Caso? casoCapturado = null;

        _casoRepoMock
            .Setup(r => r.CrearAsync(It.IsAny<Caso>()))
            .Callback<Caso>(c => casoCapturado = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.EjecutarAsync(request);

        // Assert
        casoCapturado.Should().NotBeNull();
        casoCapturado!.Titulo.Should().Be("Caso con espacios");
        casoCapturado.Descripcion.Should().Be("Descripción con espacios");

        result.Titulo.Should().Be("Caso con espacios");
        result.Descripcion.Should().Be("Descripción con espacios");

        _casoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Caso>()), Times.Once);
    }
    [Fact]
    public async Task EjecutarAsync_CasoValido_SetEaEstadoPendienteYFechaCreacion()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nombre = "Juan Perez" };

        var request = new CrearCasoRequest
        {
            Titulo = "Caso válido",
            Descripcion = "Desc",
            ClienteId = 1,
            TipoCaso = TipoCaso.Civil
        };

        _clienteRepoMock
            .Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(cliente);

        Caso? casoCapturado = null;

        _casoRepoMock
            .Setup(r => r.CrearAsync(It.IsAny<Caso>()))
            .Callback<Caso>(c => casoCapturado = c)
            .Returns(Task.CompletedTask);

        // Act
        await _service.EjecutarAsync(request);

        // Assert
        casoCapturado.Should().NotBeNull();
        casoCapturado!.Estado.Should().Be(EstadoCaso.Pendiente);
        casoCapturado.FechaCreacion.Should().NotBe(default);

        casoCapturado.ClienteId.Should().Be(1);
        casoCapturado.Cliente.Should().NotBeNull();
    }





}
