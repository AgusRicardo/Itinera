using Itinera.Application.Cargos.Dtos;

namespace Itinera.Application.Cargos.Interfaces;

public interface ICargoService
{
    Task<CargoResponse> CrearAsync(CrearCargoRequest request);
    Task<CargoResponse> ObtenerPorIdAsync(int id);
    Task<List<CargoResponse>> ObtenerTodosAsync();
    Task<CargoResponse> ActualizarAsync(int id, ActualizarCargoRequest request);
    Task EliminarAsync(int id);
}
