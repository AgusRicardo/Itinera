using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Clientes.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Services;

public class ClienteService(IClienteRepository clienteRepository) : IClienteService
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request)
    {
        var cliente = new Cliente(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono);

        await _clienteRepository.AddAsync(cliente);

        return ClienteMapper.ToResponse(cliente);
    }

    public async Task<ClienteResponse> ObtenerPorIdAsync(int id)
    {
        var cliente = await ObtenerClienteById(id);

        return ClienteMapper.ToResponse(cliente);
    }

    public async Task<List<ClienteResponse>> ObtenerTodosAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();

        return ClienteMapper.ToResponse(clientes);
    }

    public async Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request)
    {
        var cliente = await ObtenerClienteById(id);

        cliente.ActualizarDatos(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono);

        await _clienteRepository.UpdateAsync(cliente);

        return ClienteMapper.ToResponse(cliente);
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await ObtenerClienteById(id);

        cliente.Desactivar();

        await _clienteRepository.DeleteAsync(cliente);
    }

    private async Task<Cliente> ObtenerClienteById(int id)
    {
        Cliente? cliente = await _clienteRepository.GetByIdAsync(id);

        return cliente ?? throw new ClienteNoEncontradoException(id);
    }
}
