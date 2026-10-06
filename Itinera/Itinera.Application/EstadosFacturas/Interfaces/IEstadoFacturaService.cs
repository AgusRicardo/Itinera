using Itinera.Application.EstadosFacturas.Dtos;

namespace Itinera.Application.EstadosFacturas.Interfaces;

public interface IEstadoFacturaService
{
    Task<List<EstadoFacturaResponse>> ObtenerTodosAsync();
    Task<EstadoFacturaResponse> ObtenerPorIdAsync(int id);
}
