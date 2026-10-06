using Itinera.Application.Common.Exceptions;
using Itinera.Application.Paises.Dtos;
using Itinera.Application.Paises.Interfaces;
using Itinera.Application.Paises.Services;
using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Paises;

public class PaisServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_el_pais()
    {
        var repositorio = new PaisRepositoryFake();
        var servicio = new PaisService(repositorio);

        var response = await servicio.CrearAsync(new CrearPaisRequest { Nombre = "Argentina" });

        Assert.True(response.Id > 0);
        Assert.Equal("Argentina", response.Nombre);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Paises);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_pais_inexistente_lanza_excepcion()
    {
        var servicio = new PaisService(new PaisRepositoryFake());

        await Assert.ThrowsAsync<PaisNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_el_nombre()
    {
        var repositorio = new PaisRepositoryFake();
        var servicio = new PaisService(repositorio);
        var creado = await servicio.CrearAsync(new CrearPaisRequest { Nombre = "Argentina" });

        var response = await servicio.ActualizarAsync(
            creado.Id,
            new ActualizarPaisRequest { Nombre = "Chile" });

        Assert.Equal("Chile", response.Nombre);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_el_pais()
    {
        var repositorio = new PaisRepositoryFake();
        var servicio = new PaisService(repositorio);
        var creado = await servicio.CrearAsync(new CrearPaisRequest { Nombre = "Argentina" });

        await servicio.EliminarAsync(creado.Id);

        Assert.False(repositorio.Paises.Single().Activo);
    }

    [Fact]
    public async Task EliminarAsync_de_un_pais_inexistente_lanza_excepcion()
    {
        var servicio = new PaisService(new PaisRepositoryFake());

        await Assert.ThrowsAsync<PaisNoEncontradoException>(
            () => servicio.EliminarAsync(99));
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_paises_creados()
    {
        var servicio = new PaisService(new PaisRepositoryFake());
        await servicio.CrearAsync(new CrearPaisRequest { Nombre = "Argentina" });
        await servicio.CrearAsync(new CrearPaisRequest { Nombre = "Chile" });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
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
}
