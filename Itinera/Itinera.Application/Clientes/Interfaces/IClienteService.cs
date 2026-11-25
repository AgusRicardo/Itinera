using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Clientes.Dtos;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteService : IPaginadoService<ClienteResponse, ListadoActivableRequest>
{
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request);
    Task<ClienteResponse> ObtenerPorIdAsync(int id);
    Task<List<ClienteResponse>> ObtenerTodosAsync();
    Task<ResumenClientesResponse> ObtenerResumenAsync();
    Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request);
    Task EliminarAsync(int id);
}
