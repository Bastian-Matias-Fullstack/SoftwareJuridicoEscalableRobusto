
using Aplicacion.DTOs;
using Aplicacion.Excepciones;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Aplicacion.Servicios.Casos;
using Dominio.Entidades;
using FluentAssertions;
using Moq;


public class ActualizarCasoServiceTests
{
    private readonly Mock<ICasoRepository> _casoRepoMock;
    private readonly ActualizarCasoService _service;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    public ActualizarCasoServiceTests()
    {
        _casoRepoMock = new Mock<ICasoRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();

        _service = new ActualizarCasoService(
            _casoRepoMock.Object,
            _clienteRepoMock.Object
        );
    }
    [Fact]
    public async Task EjecutarAsync_CasoNoExiste_LanzaNotFoundException()
    {
        // Arrange
        var request = new ActualizarCasoRequest
        {
            ClienteId = 1,
            Titulo = "Intento inválido",
            Descripcion = "No debería permitir",
            TipoCaso = TipoCaso.Civil

        };
             _casoRepoMock
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new NotFoundException("No existe un caso con id 99."));
        // Act
        Func<Task> act = async () =>
        await _service.EjecutarAsync(99, request, esAdmin: false);

        await act.Should().ThrowAsync<NotFoundException>();

        _casoRepoMock.Verify(
            r => r.ActualizarAsync(It.IsAny<Caso>()),
            Times.Never
            );
    }
    [Fact]
    public async Task EjecutarAsync_CasoCerrado_LanzaBusinessConflictException()
    {
        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.Cerrado,
            ClienteId = 1
        };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 1,
            Titulo = "Intento inválido",
            Descripcion = "No debería permitir",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        Func<Task> act = async () => await _service.EjecutarAsync(1, request, esAdmin: false);

        await act.Should().ThrowAsync<BusinessConflictException>();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_CasoPendiente_CambiaAEnProceso()
    {
        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.Pendiente,
            ClienteId = 1
        };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 1,
            Titulo = "Título actualizado",
            Descripcion = "Descripción actualizada",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        await _service.EjecutarAsync(1, request, esAdmin: false);

        caso.Estado.Should().Be(EstadoCaso.EnProceso);
        caso.FechaCambioEstado.Should().NotBeNull();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Once);
    }

    [Fact]
    public async Task EjecutarAsync_CasoEnProceso_NoAdmin_SoloActualizaDescripcion()
    {
        // Arrange
        var fechaOriginal = DateTime.UtcNow.AddDays(-1);

        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.EnProceso,
            ClienteId = 1,
            Titulo = "Titulo original",
            TipoCaso = TipoCaso.Civil,
            Descripcion = "Desc original",
            FechaCambioEstado = fechaOriginal
        };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 999, // aunque venga distinto, NO debe cambiar por return temprano
            Titulo = "Nuevo título",
            Descripcion = "Nueva descripción",
            TipoCaso = TipoCaso.Penal
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        // Act
        await _service.EjecutarAsync(1, request, esAdmin: false);

        // Assert
        caso.Estado.Should().Be(EstadoCaso.EnProceso);
        caso.FechaCambioEstado.Should().Be(fechaOriginal);

        caso.Descripcion.Should().Be("Nueva descripción");  // SÍ cambia
        caso.Titulo.Should().Be("Titulo original");         // NO cambia
        caso.TipoCaso.Should().Be(TipoCaso.Civil);          // NO cambia
        caso.ClienteId.Should().Be(1);                      // NO cambia

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Once);
        _clienteRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        _casoRepoMock.Verify(r => r.ExisteCasoActivoParaClienteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_ClienteIdInvalido_LanzaArgumentException()
    {
        // Arrange
        var caso = new Caso { Id = 1, Estado = EstadoCaso.Pendiente, ClienteId = 1 };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 0,
            Titulo = "Titulo",
            Descripcion = "Desc",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1, request, esAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }
    [Fact]
    public async Task EjecutarAsync_TituloVacio_LanzaArgumentException()
    {
        // Arrange
        var caso = new Caso { Id = 1, Estado = EstadoCaso.Pendiente, ClienteId = 1 };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 1,
            Titulo = "   ",
            Descripcion = "Desc",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1, request, esAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }
    [Fact]
    public async Task EjecutarAsync_CasoCerrado_Admin_PermiteEditar()
    {
        // Arrange
        var caso = new Caso
        {
            Id = 1,
            Estado = EstadoCaso.Cerrado,
            ClienteId = 1,
            Titulo = "Antes",
            Descripcion = "Antes",
            TipoCaso = TipoCaso.Civil
        };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 1,
            Titulo = "Despues",
            Descripcion = "Despues",
            TipoCaso = TipoCaso.Penal
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);

        // Act
        await _service.EjecutarAsync(1, request, esAdmin: true);

        // Assert
        caso.Titulo.Should().Be("Despues");
        caso.Descripcion.Should().Be("Despues");
        caso.TipoCaso.Should().Be(TipoCaso.Penal);

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Once);
    }
    [Fact]
    public async Task EjecutarAsync_CambiaCliente_ClienteNoExiste_LanzaNotFoundException()
    {
        // Arrange
        var caso = new Caso { Id = 1, Estado = EstadoCaso.Pendiente, ClienteId = 1 };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 2, // cambia
            Titulo = "Titulo",
            Descripcion = "Desc",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);
        _clienteRepoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync((Cliente?)null);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1, request, esAdmin: true);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*cliente no existe*");

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }

    [Fact]
    public async Task EjecutarAsync_CambiaCliente_ClienteTieneOtroCasoActivo_LanzaBusinessConflictException()
    {
        // Arrange
        var caso = new Caso { Id = 1, Estado = EstadoCaso.Pendiente, ClienteId = 1 };

        var request = new ActualizarCasoRequest
        {
            ClienteId = 2, // cambia
            Titulo = "Titulo",
            Descripcion = "Desc",
            TipoCaso = TipoCaso.Civil
        };

        _casoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(caso);
        _clienteRepoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(new Cliente { Id = 2 });

        _casoRepoMock
            .Setup(r => r.ExisteCasoActivoParaClienteAsync(2, 1))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.EjecutarAsync(1, request, esAdmin: true);

        // Assert
        await act.Should().ThrowAsync<BusinessConflictException>();

        _casoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Caso>()), Times.Never);
    }





}



