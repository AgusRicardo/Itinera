using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Destinos.Dtos;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Destinos.Services;
using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Destinos;

public class DestinoServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_el_destino()
    {
        var ciudadRepositorio = new CiudadRepositoryFake();
        var ciudad = await CrearCiudad(ciudadRepositorio, "Buenos Aires", "Argentina");
        var repositorio = new DestinoRepositoryFake();
        var servicio = new DestinoService(repositorio, ciudadRepositorio);

        var response = await servicio.CrearAsync(new CrearDestinoRequest { CiudadId = ciudad.Id });

        Assert.True(response.Id > 0);
        Assert.Equal(ciudad.Id, response.CiudadId);
        Assert.Equal("Buenos Aires", response.CiudadNombre);
        Assert.Equal("Argentina", response.PaisNombre);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Destinos);
    }

    [Fact]
    public async Task CrearAsync_con_ciudad_inexistente_lanza_excepcion()
    {
        var servicio = new DestinoService(new DestinoRepositoryFake(), new CiudadRepositoryFake());

        await Assert.ThrowsAsync<CiudadNoEncontradaException>(
            () => servicio.CrearAsync(new CrearDestinoRequest { CiudadId = 99 }));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_destino_inexistente_lanza_excepcion()
    {
        var servicio = new DestinoService(new DestinoRepositoryFake(), new CiudadRepositoryFake());

        await Assert.ThrowsAsync<DestinoNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_la_ciudad()
    {
        var ciudadRepositorio = new CiudadRepositoryFake();
        var ciudad = await CrearCiudad(ciudadRepositorio, "Buenos Aires", "Argentina");
        var otraCiudad = await CrearCiudad(ciudadRepositorio, "Cordoba", "Argentina");
        var servicio = new DestinoService(new DestinoRepositoryFake(), ciudadRepositorio);
        var creado = await servicio.CrearAsync(new CrearDestinoRequest { CiudadId = ciudad.Id });

        var response = await servicio.ActualizarAsync(
            creado.Id,
            new ActualizarDestinoRequest { CiudadId = otraCiudad.Id });

        Assert.Equal(otraCiudad.Id, response.CiudadId);
        Assert.Equal("Cordoba", response.CiudadNombre);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_el_destino()
    {
        var ciudadRepositorio = new CiudadRepositoryFake();
        var ciudad = await CrearCiudad(ciudadRepositorio, "Buenos Aires", "Argentina");
        var repositorio = new DestinoRepositoryFake();
        var servicio = new DestinoService(repositorio, ciudadRepositorio);
        var creado = await servicio.CrearAsync(new CrearDestinoRequest { CiudadId = ciudad.Id });

        await servicio.EliminarAsync(creado.Id);

        Assert.False(repositorio.Destinos.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_destinos_creados()
    {
        var ciudadRepositorio = new CiudadRepositoryFake();
        var ciudad = await CrearCiudad(ciudadRepositorio, "Buenos Aires", "Argentina");
        var servicio = new DestinoService(new DestinoRepositoryFake(), ciudadRepositorio);

        await servicio.CrearAsync(new CrearDestinoRequest { CiudadId = ciudad.Id });
        await servicio.CrearAsync(new CrearDestinoRequest { CiudadId = ciudad.Id });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private static async Task<Ciudad> CrearCiudad(CiudadRepositoryFake repositorio, string nombre, string paisNombre)
    {
        var ciudad = new Ciudad(nombre, new Pais(paisNombre));
        await repositorio.AddAsync(ciudad);
        return ciudad;
    }

    private sealed class CiudadRepositoryFake : ICiudadRepository
    {
        private int _ultimoId;

        public List<Ciudad> Ciudades { get; } = new();

        public Task AddAsync(Ciudad ciudad)
        {
            _ultimoId++;
            typeof(Ciudad).GetProperty(nameof(Ciudad.Id))!.SetValue(ciudad, _ultimoId);
            Ciudades.Add(ciudad);
            return Task.CompletedTask;
        }

        public Task<Ciudad?> GetByIdAsync(int id)
            => Task.FromResult(Ciudades.FirstOrDefault(c => c.Id == id));

        public Task<List<Ciudad>> GetAllAsync()
            => Task.FromResult(Ciudades.ToList());

        public Task UpdateAsync(Ciudad ciudad) => Task.CompletedTask;

        public Task DeleteAsync(Ciudad ciudad) => Task.CompletedTask;
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

        public Task UpdateAsync(Destino destino) => Task.CompletedTask;

        public Task DeleteAsync(Destino destino) => Task.CompletedTask;
    }
}
