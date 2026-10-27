using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Clientes.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Services;

public class ClienteService(
    IClienteRepository clienteRepository,
    ITenantContext tenantContext,
    IEmpresaClienteRepository empresaClienteRepository) : IClienteService
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ITenantContext _tenantContext = tenantContext;
    private readonly IEmpresaClienteRepository _empresaClienteRepository = empresaClienteRepository;

    public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request)
    {
        var empresaId = _tenantContext.EmpresaId
            ?? throw new InvalidOperationException("No se pudo determinar la empresa del usuario.");

        var cliente = new Cliente(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono);

        await _clienteRepository.AddAsync(cliente);
        await _empresaClienteRepository.AddAsync(new EmpresaCliente(empresaId, cliente.Id));

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
