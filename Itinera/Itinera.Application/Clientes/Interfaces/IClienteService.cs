using Itinera.Application.Common.Paginacion;
using Itinera.Application.Clientes.Dtos;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteService
{
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request);
    Task<ClienteResponse> ObtenerPorIdAsync(int id);
    Task<List<ClienteResponse>> ObtenerTodosAsync();
    Task<Pagina<ClienteResponse>> ObtenerPaginaAsync(ListarClientesRequest request);
    Task<ResumenClientesResponse> ObtenerResumenAsync();
    Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request);
    Task EliminarAsync(int id);
}
