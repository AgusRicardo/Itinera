using Itinera.Application.Destinos.Dtos;

namespace Itinera.Application.Destinos.Interfaces;

public interface IDestinoService
{
    Task<DestinoResponse> CrearAsync(CrearDestinoRequest request);
    Task<DestinoResponse> ObtenerPorIdAsync(int id);
    Task<List<DestinoResponse>> ObtenerTodosAsync();
    Task<DestinoResponse> ActualizarAsync(int id, ActualizarDestinoRequest request);
    Task EliminarAsync(int id);
}
