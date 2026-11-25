using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Actividades.Dtos;

namespace Itinera.Application.Actividades.Interfaces;

public interface IActividadService : IPaginadoService<ActividadResponse, ListarActividadesRequest>
{
    Task<ActividadResponse> CrearAsync(CrearActividadRequest request);
    Task<ActividadResponse> ObtenerPorIdAsync(int id);
    Task<List<ActividadResponse>> ObtenerTodosAsync();
    Task<ActividadResponse> ActualizarAsync(int id, ActualizarActividadRequest request);
    Task EliminarAsync(int id);
}
