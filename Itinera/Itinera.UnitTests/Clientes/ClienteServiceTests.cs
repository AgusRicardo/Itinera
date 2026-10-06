using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Clientes.Services;
using Itinera.Application.Common.Exceptions;
using Itinera.Domain.Usuarios;
using Xunit;

namespace Itinera.UnitTests.Clientes;

public class ClienteServiceTests
{
    [Fact]
    public async Task CrearAsync_persiste_y_devuelve_el_cliente()
    {
        var repositorio = new ClienteRepositoryFake();
        var servicio = new ClienteService(repositorio);

        var response = await servicio.CrearAsync(new CrearClienteRequest
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Email = "ana@correo.com",
            Telefono = "1122334455"
        });

        Assert.True(response.Id > 0);
        Assert.Equal("Ana", response.Nombre);
        Assert.Equal("Gomez", response.Apellido);
        Assert.True(response.Activo);
        Assert.Single(repositorio.Clientes);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_de_un_cliente_inexistente_lanza_excepcion()
    {
        var servicio = new ClienteService(new ClienteRepositoryFake());

        await Assert.ThrowsAsync<ClienteNoEncontradoException>(
            () => servicio.ObtenerPorIdAsync(99));
    }

    [Fact]
    public async Task ActualizarAsync_cambia_los_datos()
    {
        var repositorio = new ClienteRepositoryFake();
        var servicio = new ClienteService(repositorio);
        var creado = await servicio.CrearAsync(new CrearClienteRequest
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Email = "ana@correo.com",
            Telefono = "1122334455"
        });

        var response = await servicio.ActualizarAsync(creado.Id, new ActualizarClienteRequest
        {
            Nombre = "Ana",
            Apellido = "Perez",
            Email = "ana.perez@correo.com",
            Telefono = "1199887766"
        });

        Assert.Equal("Perez", response.Apellido);
        Assert.Equal("ana.perez@correo.com", response.Email);
    }

    [Fact]
    public async Task EliminarAsync_desactiva_el_cliente()
    {
        var repositorio = new ClienteRepositoryFake();
        var servicio = new ClienteService(repositorio);
        var creado = await servicio.CrearAsync(new CrearClienteRequest
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Email = "ana@correo.com",
            Telefono = "1122334455"
        });

        await servicio.EliminarAsync(creado.Id);

        Assert.False(repositorio.Clientes.Single().Activo);
    }

    [Fact]
    public async Task ObtenerTodosAsync_devuelve_los_clientes_creados()
    {
        var servicio = new ClienteService(new ClienteRepositoryFake());
        await servicio.CrearAsync(new CrearClienteRequest { Nombre = "Ana", Apellido = "Gomez", Email = "ana@correo.com" });
        await servicio.CrearAsync(new CrearClienteRequest { Nombre = "Luis", Apellido = "Diaz", Email = "luis@correo.com" });

        var response = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, response.Count);
    }

    private sealed class ClienteRepositoryFake : IClienteRepository
    {
        private int _ultimoId;

        public List<Cliente> Clientes { get; } = new();

        public Task AddAsync(Cliente cliente)
        {
            _ultimoId++;
            typeof(Cliente).GetProperty(nameof(Cliente.Id))!.SetValue(cliente, _ultimoId);
            Clientes.Add(cliente);
            return Task.CompletedTask;
        }

        public Task<Cliente?> GetByIdAsync(int id)
            => Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));

        public Task<List<Cliente>> GetAllAsync()
            => Task.FromResult(Clientes.ToList());

        public Task UpdateAsync(Cliente cliente) => Task.CompletedTask;

        public Task DeleteAsync(Cliente cliente) => Task.CompletedTask;
    }
}
