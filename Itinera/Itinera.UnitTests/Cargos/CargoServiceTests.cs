using Itinera.Application.Cargos.Dtos;
using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Cargos.Services;
using Itinera.Application.Common.Exceptions;
using Itinera.Domain.Empresa;
using Xunit;

namespace Itinera.UnitTests.Cargos;

public class CargoServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_el_cargo()
    {
        var repositorio = new CargoRepositoryFake();
        var servicio = new CargoService(repositorio);

        var response = await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente de viajes" });

        Assert.True(response.Id > 0);
        Assert.Equal("Agente de viajes", response.Descripcion);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Cargos);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_cargo_inexistente_lanza_excepcion()
    {
        var servicio = new CargoService(new CargoRepositoryFake());

        await Assert.ThrowsAsync<CargoNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_la_descripcion()
    {
        var repositorio = new CargoRepositoryFake();
        var servicio = new CargoService(repositorio);
        var creado = await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente de viajes" });

        var response = await servicio.ActualizarAsync(
            creado.Id,
            new ActualizarCargoRequest { Descripcion = "Coordinador" });

        Assert.Equal("Coordinador", response.Descripcion);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_el_cargo()
    {
        var repositorio = new CargoRepositoryFake();
        var servicio = new CargoService(repositorio);
        var creado = await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente de viajes" });

        await servicio.EliminarAsync(creado.Id);

        Assert.False(repositorio.Cargos.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_cargos_creados()
    {
        var servicio = new CargoService(new CargoRepositoryFake());
        await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente de viajes" });
        await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Coordinador" });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task No_admite_descripciones_repetidas_salvo_que_el_otro_este_desactivado()
    {
        var servicio = new CargoService(new CargoRepositoryFake());
        var agente = await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente" });
        var otro = await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Coordinador" });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => servicio.CrearAsync(new CrearCargoRequest { Descripcion = " AGENTE " }));
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => servicio.ActualizarAsync(otro.Id, new ActualizarCargoRequest { Descripcion = "agente" }));
        await servicio.ActualizarAsync(agente.Id, new ActualizarCargoRequest { Descripcion = "agente" });

        await servicio.EliminarAsync(agente.Id);
        await servicio.CrearAsync(new CrearCargoRequest { Descripcion = "Agente" });
    }

    private sealed class CargoRepositoryFake : ICargoRepository
    {
        private int _ultimoId;

        public List<Cargo> Cargos { get; } = new();

        public Task AddAsync(Cargo cargo)
        {
            _ultimoId++;
            typeof(Cargo).GetProperty(nameof(Cargo.Id))!.SetValue(cargo, _ultimoId);
            Cargos.Add(cargo);
            return Task.CompletedTask;
        }

        public Task<Cargo?> GetByIdAsync(int id)
            => Task.FromResult(Cargos.FirstOrDefault(c => c.Id == id));

        public Task<bool> ExisteAsync(string descripcion, int? idExcluido)
            => Task.FromResult(Cargos.Any(c => c.Activo && c.Id != idExcluido && string.Equals(c.Descripcion, descripcion, StringComparison.OrdinalIgnoreCase)));

        public Task<List<Cargo>> GetAllAsync()
            => Task.FromResult(Cargos.ToList());

        public Task UpdateAsync(Cargo cargo) => Task.CompletedTask;

        public Task DeleteAsync(Cargo cargo) => Task.CompletedTask;
    }
}
