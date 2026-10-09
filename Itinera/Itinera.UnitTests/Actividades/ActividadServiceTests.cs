using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Actividades.Services;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Actividades;

public class ActividadServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_la_actividad()
    {
        var destinoRepositorio = new DestinoRepositoryFake();
        var destino = await CrearDestino(destinoRepositorio);
        var repositorio = new ActividadRepositoryFake();
        var servicio = new ActividadService(repositorio, destinoRepositorio);

        var response = await servicio.CrearAsync(new CrearActividadRequest
        {
            Nombre = "Tour",
            Descripcion = "Recorrido guiado",
            CostoBase = 1500m,
            DuracionEstimada = 120,
            DestinoId = destino.Id
        });

        Assert.True(response.Id > 0);
        Assert.Equal("Tour", response.Nombre);
        Assert.Equal(destino.Id, response.DestinoId);
        Assert.Equal("Buenos Aires", response.CiudadNombre);
        Assert.Equal("Argentina", response.PaisNombre);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Actividades);
    }

    [Fact]
    public async Task CrearAsync_con_destino_inexistente_lanza_excepcion()
    {
        var servicio = new ActividadService(new ActividadRepositoryFake(), new DestinoRepositoryFake());

        await Assert.ThrowsAsync<DestinoNoEncontradoException>(
            () => servicio.CrearAsync(new CrearActividadRequest
            {
                Nombre = "Tour",
                CostoBase = 100m,
                DuracionEstimada = 60,
                DestinoId = 99
            }));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_una_actividad_inexistente_lanza_excepcion()
    {
        var servicio = new ActividadService(new ActividadRepositoryFake(), new DestinoRepositoryFake());

        await Assert.ThrowsAsync<ActividadNoEncontradaException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_los_datos()
    {
        var destinoRepositorio = new DestinoRepositoryFake();
        var destino = await CrearDestino(destinoRepositorio);
        var servicio = new ActividadService(new ActividadRepositoryFake(), destinoRepositorio);
        var creada = await servicio.CrearAsync(new CrearActividadRequest
        {
            Nombre = "Tour",
            CostoBase = 100m,
            DuracionEstimada = 60,
            DestinoId = destino.Id
        });

        var response = await servicio.ActualizarAsync(creada.Id, new ActualizarActividadRequest
        {
            Nombre = "City tour",
            Descripcion = "nueva",
            CostoBase = 250m,
            DuracionEstimada = 90,
            DestinoId = destino.Id
        });

        Assert.Equal("City tour", response.Nombre);
        Assert.Equal(250m, response.CostoBase);
        Assert.Equal(90, response.DuracionEstimada);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_la_actividad()
    {
        var destinoRepositorio = new DestinoRepositoryFake();
        var destino = await CrearDestino(destinoRepositorio);
        var repositorio = new ActividadRepositoryFake();
        var servicio = new ActividadService(repositorio, destinoRepositorio);
        var creada = await servicio.CrearAsync(new CrearActividadRequest
        {
            Nombre = "Tour",
            CostoBase = 100m,
            DuracionEstimada = 60,
            DestinoId = destino.Id
        });

        await servicio.EliminarAsync(creada.Id);

        Assert.False(repositorio.Actividades.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_las_actividades_creadas()
    {
        var destinoRepositorio = new DestinoRepositoryFake();
        var destino = await CrearDestino(destinoRepositorio);
        var servicio = new ActividadService(new ActividadRepositoryFake(), destinoRepositorio);

        await servicio.CrearAsync(new CrearActividadRequest { Nombre = "Tour", CostoBase = 100m, DuracionEstimada = 60, DestinoId = destino.Id });
        await servicio.CrearAsync(new CrearActividadRequest { Nombre = "Museo", CostoBase = 200m, DuracionEstimada = 30, DestinoId = destino.Id });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private static async Task<Destino> CrearDestino(DestinoRepositoryFake repositorio)
    {
        var destino = new Destino(new Ciudad("Buenos Aires", new Pais("Argentina")));
        await repositorio.AddAsync(destino);
        return destino;
    }

    private sealed class DestinoRepositoryFake : IDestinoRepository
    {
        private int _ultimoId;

        public List<Destino> Destinos { get; } = new();

        public Task AddAsync(Destino destino)
        {
            _ultimoId++;
            typeof(Destino).GetProperty(nameof(Destino.Id))!.SetValue(destino, _ultimoId);
            Destinos.Add(destino);
            return Task.CompletedTask;
        }

        public Task<Destino?> GetByIdAsync(int id)
            => Task.FromResult(Destinos.FirstOrDefault(d => d.Id == id));

        public Task<List<Destino>> GetAllAsync()
            => Task.FromResult(Destinos.ToList());

        public Task<List<Destino>> ObtenerPorIdsConActividadesAsync(IEnumerable<int> ids)
        {
            var listaIds = ids.ToList();
            return Task.FromResult(Destinos.Where(destino => listaIds.Contains(destino.Id)).ToList());
        }

        public Task UpdateAsync(Destino destino) => Task.CompletedTask;

        public Task DeleteAsync(Destino destino) => Task.CompletedTask;
    }

    private sealed class ActividadRepositoryFake : IActividadRepository
    {
        private int _ultimoId;

        public List<Actividad> Actividades { get; } = new();

        public Task AddAsync(Actividad actividad)
        {
            _ultimoId++;
            typeof(Actividad).GetProperty(nameof(Actividad.Id))!.SetValue(actividad, _ultimoId);
            Actividades.Add(actividad);
            return Task.CompletedTask;
        }

        public Task<Actividad?> GetByIdAsync(int id)
            => Task.FromResult(Actividades.FirstOrDefault(a => a.Id == id));

        public Task<List<Actividad>> GetAllAsync()
            => Task.FromResult(Actividades.ToList());

        public Task UpdateAsync(Actividad actividad) => Task.CompletedTask;

        public Task DeleteAsync(Actividad actividad) => Task.CompletedTask;
    }
}
