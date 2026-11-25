using Itinera.Application.Etiquetas.Dtos;

namespace Itinera.Application.Etiquetas.Interfaces;

public interface IEtiquetaService
{
    Task<EtiquetaResponse> CrearAsync(CrearEtiquetaRequest request);
    Task<EtiquetaResponse> ObtenerPorIdAsync(int id);
    Task<List<EtiquetaResponse>> ObtenerTodosAsync();
    Task<EtiquetaResponse> ActualizarAsync(int id, ActualizarEtiquetaRequest request);
    Task EliminarAsync(int id);
}
