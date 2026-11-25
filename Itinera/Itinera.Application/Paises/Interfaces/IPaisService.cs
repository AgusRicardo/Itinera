using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Paises.Dtos;

namespace Itinera.Application.Paises.Interfaces;

public interface IPaisService : IPaginadoService<PaisResponse, ListadoRequest>
{
    Task<PaisResponse> CrearAsync(CrearPaisRequest request);
    Task<PaisResponse> ObtenerPorIdAsync(int id);
    Task<List<PaisResponse>> ObtenerTodosAsync();
    Task<PaisResponse> ActualizarAsync(int id, ActualizarPaisRequest request);
    Task EliminarAsync(int id);
}
