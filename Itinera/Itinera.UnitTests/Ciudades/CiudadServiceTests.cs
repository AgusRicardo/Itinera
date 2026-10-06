using Itinera.Application.Ciudades.Dtos;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Ciudades.Services;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Paises.Interfaces;
using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Ciudades;

public class CiudadServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_la_ciudad()
    {
        var paisRepositorio = new PaisRepositoryFake();
        var pais = await CrearPais(paisRepositorio, "Argentina");
        var repositorio = new CiudadRepositoryFake();
        var servicio = new CiudadService(repositorio, paisRepositorio);

        var response = await servicio.CrearAsync(new CrearCiudadRequest
        {
            Nombre = "Buenos Aires",
            PaisId = pais.Id
        });

        Assert.True(response.Id > 0);
        Assert.Equal("Buenos Aires", response.Nombre);
        Assert.Equal(pais.Id, response.PaisId);
        Assert.Equal("Argentina", response.PaisNombre);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Ciudades);
    }

    [Fact]
    public async Task CrearAsync_con_pais_inexistente_lanza_excepcion()
    {
        var servicio = new CiudadService(new CiudadRepositoryFake(), new PaisRepositoryFake());

        await Assert.ThrowsAsync<PaisNoEncontradoException>(
            () => servicio.CrearAsync(new CrearCiudadRequest { Nombre = "Buenos Aires", PaisId = 99 }));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_una_ciudad_inexistente_lanza_excepcion()
    {
        var servicio = new CiudadService(new CiudadRepositoryFake(), new PaisRepositoryFake());

        await Assert.ThrowsAsync<CiudadNoEncontradaException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_nombre_y_pais()
    {
        var paisRepositorio = new PaisRepositoryFake();
        var argentina = await CrearPais(paisRepositorio, "Argentina");
        var chile = await CrearPais(paisRepositorio, "Chile");
        var servicio = new CiudadService(new CiudadRepositoryFake(), paisRepositorio);
        var creada = await servicio.CrearAsync(new CrearCiudadRequest
        {
            Nombre = "Buenos Aires",
            PaisId = argentina.Id
        });

        var response = await servicio.ActualizarAsync(creada.Id, new ActualizarCiudadRequest
        {
            Nombre = "Santiago",
            PaisId = chile.Id
        });

        Assert.Equal("Santiago", response.Nombre);
        Assert.Equal(chile.Id, response.PaisId);
        Assert.Equal("Chile", response.PaisNombre);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_la_ciudad()
    {
        var paisRepositorio = new PaisRepositoryFake();
        var pais = await CrearPais(paisRepositorio, "Argentina");
        var repositorio = new CiudadRepositoryFake();
        var servicio = new CiudadService(repositorio, paisRepositorio);
        var creada = await servicio.CrearAsync(new CrearCiudadRequest
        {
            Nombre = "Buenos Aires",
            PaisId = pais.Id
        });

        await servicio.EliminarAsync(creada.Id);

        Assert.False(repositorio.Ciudades.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_las_ciudades_creadas()
    {
        var paisRepositorio = new PaisRepositoryFake();
        var pais = await CrearPais(paisRepositorio, "Argentina");
        var servicio = new CiudadService(new CiudadRepositoryFake(), paisRepositorio);

        await servicio.CrearAsync(new CrearCiudadRequest { Nombre = "Buenos Aires", PaisId = pais.Id });
        await servicio.CrearAsync(new CrearCiudadRequest { Nombre = "Cordoba", PaisId = pais.Id });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private static async Task<Pais> CrearPais(PaisRepositoryFake repositorio, string nombre)
    {
        var pais = new Pais(nombre);
        await repositorio.AddAsync(pais);
        return pais;
    }

    private sealed class PaisRepositoryFake : IPaisRepository
    {
        private int _ultimoId;

        public List<Pais> Paises { get; } = new();

        public Task AddAsync(Pais pais)
        {
            _ultimoId++;
            typeof(Pais).GetProperty(nameof(Pais.Id))!.SetValue(pais, _ultimoId);
            Paises.Add(pais);
            return Task.CompletedTask;
        }

        public Task<Pais?> GetByIdAsync(int id)
            => Task.FromResult(Paises.FirstOrDefault(p => p.Id == id));

        public Task<List<Pais>> GetAllAsync()
            => Task.FromResult(Paises.ToList());

        public Task UpdateAsync(Pais pais) => Task.CompletedTask;

        public Task DeleteAsync(Pais pais) => Task.CompletedTask;
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
}
