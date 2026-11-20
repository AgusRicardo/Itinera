using Itinera.Application.Etiquetas.Dtos;

namespace Itinera.Application.Etiquetas.Interfaces;

public interface IEtiquetaService
{
    Task<EtiquetaResponse> CrearAsync(CrearEtiquetaRequest request);
    Task<List<EtiquetaResponse>> ObtenerTodasAsync();
    Task<EtiquetaResponse> ActualizarAsync(int id, CrearEtiquetaRequest request);
    Task DesactivarAsync(int id);
}
