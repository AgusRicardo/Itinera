using Itinera.Application.Common.Exceptions;
using Itinera.Application.EstadosFacturas.Interfaces;
using Itinera.Application.EstadosFacturas.Services;
using Itinera.Domain.Common;
using Xunit;

namespace Itinera.UnitTests.EstadosFacturas;

public class EstadoFacturaServiceTests
{
    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_estados()
    {
        var repositorio = new EstadoFacturaRepositoryFake();
        repositorio.Agregar(CrearEstado(1, "Pendiente"));
        repositorio.Agregar(CrearEstado(2, "Pagada"));
        var servicio = new EstadoFacturaService(repositorio);

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_devuelve_el_estado()
    {
        var repositorio = new EstadoFacturaRepositoryFake();
        repositorio.Agregar(CrearEstado(1, "Pendiente"));
        var servicio = new EstadoFacturaService(repositorio);

        var response = await servicio.ObtenerPorIdAsync(1);

        Assert.Equal(1, response.Id);
        Assert.Equal("Pendiente", response.Descripcion);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_estado_inexistente_lanza_excepcion()
    {
        var servicio = new EstadoFacturaService(new EstadoFacturaRepositoryFake());

        await Assert.ThrowsAsync<EstadoFacturaNoEncontradaException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    private static EstadoFacturaCatalogo CrearEstado(int id, string descripcion)
    {
        var estado = (EstadoFacturaCatalogo)Activator.CreateInstance(
            typeof(EstadoFacturaCatalogo), nonPublic: true)!;
        typeof(EstadoFacturaCatalogo).GetProperty(nameof(EstadoFacturaCatalogo.Id))!.SetValue(estado, id);
        typeof(EstadoFacturaCatalogo).GetProperty(nameof(EstadoFacturaCatalogo.Descripcion))!.SetValue(estado, descripcion);
        return estado;
    }

    private sealed class EstadoFacturaRepositoryFake : IEstadoFacturaRepository
    {
        private readonly List<EstadoFacturaCatalogo> _estados = new();

        public void Agregar(EstadoFacturaCatalogo estado) => _estados.Add(estado);

        public Task<List<EstadoFacturaCatalogo>> GetAllAsync()
            => Task.FromResult(_estados.ToList());

        public Task<EstadoFacturaCatalogo?> GetByIdAsync(int id)
            => Task.FromResult(_estados.FirstOrDefault(e => e.Id == id));
    }
}
