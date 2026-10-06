using Itinera.Application.EstadosPropuestas.Dtos;

namespace Itinera.Application.EstadosPropuestas.Interfaces;

public interface IEstadoPropuestaService
{
    Task<List<EstadoPropuestaResponse>> ObtenerTodosAsync();
    Task<EstadoPropuestaResponse> ObtenerPorIdAsync(int id);
}
