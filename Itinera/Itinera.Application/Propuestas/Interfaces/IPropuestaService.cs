using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Dtos;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IPropuestaService : IPaginadoService<PropuestaResponse, ListadoComercialRequest>
{
    Task<PropuestaResponse> CrearAsync(CrearPropuestaRequest request);
    Task<PropuestaResponse> ActualizarAsync(int id, ActualizarPropuestaRequest request);
    Task<PropuestaResponse> ObtenerPorIdAsync(int id);
    Task<List<PropuestaResponse>> ObtenerTodosAsync();
    Task<ResumenPropuestasResponse> ObtenerResumenAsync();
    Task<List<PropuestaResponse>> ObtenerPorClienteAsync(int clienteId);
    Task<PropuestaResponse> PresentarAsync(int id);
    Task<PropuestaResponse> RegistrarRespuestaAsync(int id, RegistrarRespuestaRequest request);
    Task<PropuestaResponse> GuardarItinerarioAsync(int id, int destinoId, GuardarItinerarioRequest request);
    Task EliminarAsync(int id);
}
