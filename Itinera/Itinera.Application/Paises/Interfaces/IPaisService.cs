using Itinera.Application.Paises.Dtos;

namespace Itinera.Application.Paises.Interfaces;

public interface IPaisService
{
    Task<PaisResponse> CrearAsync(CrearPaisRequest request);
    Task<PaisResponse> ObtenerPorIdAsync(int id);
    Task<List<PaisResponse>> ObtenerTodosAsync();
    Task<PaisResponse> ActualizarAsync(int id, ActualizarPaisRequest request);
    Task EliminarAsync(int id);
}
