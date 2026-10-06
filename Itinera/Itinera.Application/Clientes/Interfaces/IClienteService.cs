using Itinera.Application.Clientes.Dtos;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteService
{
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request);
    Task<ClienteResponse> ObtenerPorIdAsync(int id);
    Task<List<ClienteResponse>> ObtenerTodosAsync();
    Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request);
    Task EliminarAsync(int id);
}
