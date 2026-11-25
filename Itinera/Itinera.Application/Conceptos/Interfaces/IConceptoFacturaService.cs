using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Conceptos.Dtos;

namespace Itinera.Application.Conceptos.Interfaces;

public interface IConceptoFacturaService : IPaginadoService<ConceptoFacturaResponse, ListadoRequest>
{
    Task<ConceptoFacturaResponse> ObtenerPorIdAsync(int id);
    Task<List<ConceptoFacturaResponse>> ObtenerTodosAsync();
    Task<ConceptoFacturaResponse> CrearAsync(CrearConceptoFacturaRequest request);
    Task<ConceptoFacturaResponse> ActualizarAsync(int id, ActualizarConceptoFacturaRequest request);
    Task EliminarAsync(int id);
}
