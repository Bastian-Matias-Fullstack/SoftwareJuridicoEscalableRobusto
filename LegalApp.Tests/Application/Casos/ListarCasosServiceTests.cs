using Aplicacion.DTOs;
using Aplicacion.Repositorio;
using Aplicacion.Servicios.Casos;
using Dominio.Entidades;
using Infraestructura.Persistencia;
using Infraestructura.Repositorios;
using Microsoft.EntityFrameworkCore;

public class ListarCasosServiceTests
{
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static void SeedCasos(AppDbContext context)
    {
        var clienteJuan = new Cliente("1-9", "Juan Perez");
        var clienteMaria = new Cliente("2-7", "Maria Lopez");

        context.Clientes.AddRange(clienteJuan, clienteMaria);

        context.Casos.AddRange(
            new Caso
            {
                Titulo = "Caso Pendiente Juan",
                Estado = EstadoCaso.Pendiente,
                Cliente = clienteJuan,
                TipoCaso = TipoCaso.Civil
            },
            new Caso
            {
                Titulo = "Caso Cerrado Juan",
                Estado = EstadoCaso.Cerrado,
                Cliente = clienteJuan,
                TipoCaso = TipoCaso.Civil
            },
            new Caso
            {
                Titulo = "Caso Pendiente Maria",
                Estado = EstadoCaso.Pendiente,
                Cliente = clienteMaria,
                TipoCaso = TipoCaso.Penal
            }
        );

        context.SaveChanges();
    }

    [Fact]
    public async Task EjecutarAsync_AplicaFiltroPorEstadoYBusqueda()
    {
        // Arrange
        using var context = CrearContexto();
        SeedCasos(context);

        ICasoRepository repository = new CasoRepository(context);
        var service = new ListarCasosService(repository);

        var filtro = new FiltroCasosRequest
        {
            Estado = "Pendiente",
            Buscar = "Juan"
        };

        // Act
        var resultado = await service.EjecutarAsync(filtro);

        // Assert
        Assert.Single(resultado.Items);
        Assert.Equal("Caso Pendiente Juan", resultado.Items[0].Titulo);
    }

    [Fact]
    public async Task EjecutarAsync_AplicaPaginacionCorrectamente()
    {
        // Arrange
        using var context = CrearContexto();

        var cliente = new Cliente("3-3", "Cliente Test");
        context.Clientes.Add(cliente);

        for (int i = 1; i <= 25; i++)
        {
            context.Casos.Add(new Caso
            {
                Titulo = $"Caso {i}",
                Estado = EstadoCaso.Pendiente,
                Cliente = cliente,
                TipoCaso = TipoCaso.Civil
            });
        }

        context.SaveChanges();

        ICasoRepository repository = new CasoRepository(context);
        var service = new ListarCasosService(repository);

        var filtro = new FiltroCasosRequest
        {
            Pagina = 2,
            Tamanio = 10
        };

        // Act
        var resultado = await service.EjecutarAsync(filtro);

        // Assert
        Assert.Equal(10, resultado.Items.Count);
        Assert.Equal(25, resultado.TotalRegistros);
        Assert.Equal(2, resultado.Pagina);
        Assert.Equal(3, resultado.TotalPaginas);
    }

    [Fact]
    public async Task EjecutarAsync_CalculaResumenCorrectamente()
    {
        // Arrange
        using var context = CrearContexto();

        var cliente = new Cliente("4-4", "Cliente Resumen");
        context.Clientes.Add(cliente);

        context.Casos.AddRange(
            new Caso { Estado = EstadoCaso.Pendiente, Cliente = cliente, TipoCaso = TipoCaso.Civil },
            new Caso { Estado = EstadoCaso.Pendiente, Cliente = cliente, TipoCaso = TipoCaso.Civil },
            new Caso { Estado = EstadoCaso.Cerrado, Cliente = cliente, TipoCaso = TipoCaso.Penal }
        );

        context.SaveChanges();

        ICasoRepository repository = new CasoRepository(context);
        var service = new ListarCasosService(repository);

        var filtro = new FiltroCasosRequest();

        // Act
        var resultado = await service.EjecutarAsync(filtro);

        // Assert
        // Assert
        Assert.NotNull(resultado.Resumen);
        Assert.Equal(3, resultado.Resumen.Total);
        Assert.Equal(2, resultado.Resumen.Pendientes);
        Assert.Equal(1, resultado.Resumen.Resueltos);

        Assert.Equal(3, resultado.TotalRegistros);
        Assert.Equal(1, resultado.TotalPaginas);
    }
    [Fact]
    public async Task EjecutarAsync_OrdenaPorTituloAsc()
    {
        using var context = CrearContexto();

        var cliente = new Cliente("5-5", "Cliente Orden");
        context.Clientes.Add(cliente);

        context.Casos.AddRange(
            new Caso { Titulo = "Zeta", Estado = EstadoCaso.Pendiente, Cliente = cliente, TipoCaso = TipoCaso.Civil },
            new Caso { Titulo = "Alfa", Estado = EstadoCaso.Pendiente, Cliente = cliente, TipoCaso = TipoCaso.Civil },
            new Caso { Titulo = "Beta", Estado = EstadoCaso.Pendiente, Cliente = cliente, TipoCaso = TipoCaso.Civil }
        );
        context.SaveChanges();

        ICasoRepository repository = new CasoRepository(context);
        var service = new ListarCasosService(repository);

        var filtro = new FiltroCasosRequest { Orden = "titulo_asc", Pagina = 1, Tamanio = 10 };

        var resultado = await service.EjecutarAsync(filtro);

        Assert.Equal(new[] { "Alfa", "Beta", "Zeta" }, resultado.Items.Select(i => i.Titulo).ToArray());
    }

}