using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Ciudades.Dtos;

namespace Itinera.Application.Ciudades.Interfaces;

public interface ICiudadService : IPaginadoService<CiudadResponse, ListadoRequest>
{
    Task<CiudadResponse> CrearAsync(CrearCiudadRequest request);
    Task<CiudadResponse> ObtenerPorIdAsync(int id);
    Task<List<CiudadResponse>> ObtenerTodosAsync();
    Task<CiudadResponse> ActualizarAsync(int id, ActualizarCiudadRequest request);
    Task EliminarAsync(int id);
}
