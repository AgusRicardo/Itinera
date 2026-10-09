using Itinera.Application.Propuestas.Dtos;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IPropuestaService
{
    Task<PropuestaResponse> CrearAsync(CrearPropuestaRequest request);
    Task<PropuestaResponse> ObtenerPorIdAsync(int id);
    Task<List<PropuestaResponse>> ObtenerPorClienteAsync(int clienteId);
    Task<ItinerarioResponse> ObtenerItinerarioAsync(int propuestaId);
    Task<PropuestaResponse> PresentarAsync(int id);
    Task<PropuestaResponse> RegistrarRespuestaAsync(int id, RegistrarRespuestaRequest request);
    Task<PropuestaResponse> ModificarItinerarioAsync(int id, ModificarItinerarioRequest request);
}
