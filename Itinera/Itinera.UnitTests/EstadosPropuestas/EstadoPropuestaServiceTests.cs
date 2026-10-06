using Itinera.Application.Common.Exceptions;
using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Application.EstadosPropuestas.Services;
using Itinera.Domain.Common;
using Xunit;

namespace Itinera.UnitTests.EstadosPropuestas;

public class EstadoPropuestaServiceTests
{
    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_estados()
    {
        var repositorio = new EstadoPropuestaRepositoryFake();
        repositorio.Agregar(CrearEstado(1, "Borrador"));
        repositorio.Agregar(CrearEstado(2, "Presentada"));
        var servicio = new EstadoPropuestaService(repositorio);

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_devuelve_el_estado()
    {
        var repositorio = new EstadoPropuestaRepositoryFake();
        repositorio.Agregar(CrearEstado(1, "Borrador"));
        var servicio = new EstadoPropuestaService(repositorio);

        var response = await servicio.ObtenerPorIdAsync(1);

        Assert.Equal(1, response.Id);
        Assert.Equal("Borrador", response.Descripcion);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_estado_inexistente_lanza_excepcion()
    {
        var servicio = new EstadoPropuestaService(new EstadoPropuestaRepositoryFake());

        await Assert.ThrowsAsync<EstadoPropuestaNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    private static EstadoPropuestaCatalogo CrearEstado(int id, string descripcion)
    {
        var estado = (EstadoPropuestaCatalogo)Activator.CreateInstance(
            typeof(EstadoPropuestaCatalogo), nonPublic: true)!;
        typeof(EstadoPropuestaCatalogo).GetProperty(nameof(EstadoPropuestaCatalogo.Id))!.SetValue(estado, id);
        typeof(EstadoPropuestaCatalogo).GetProperty(nameof(EstadoPropuestaCatalogo.Descripcion))!.SetValue(estado, descripcion);
        return estado;
    }

    private sealed class EstadoPropuestaRepositoryFake : IEstadoPropuestaRepository
    {
        private readonly List<EstadoPropuestaCatalogo> _estados = new();

        public void Agregar(EstadoPropuestaCatalogo estado) => _estados.Add(estado);

        public Task<List<EstadoPropuestaCatalogo>> GetAllAsync()
            => Task.FromResult(_estados.ToList());

        public Task<EstadoPropuestaCatalogo?> GetByIdAsync(int id)
            => Task.FromResult(_estados.FirstOrDefault(e => e.Id == id));
    }
}
