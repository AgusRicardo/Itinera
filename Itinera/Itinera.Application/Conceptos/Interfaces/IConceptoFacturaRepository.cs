using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Facturacion;

namespace Itinera.Application.Conceptos.Interfaces;

public interface IConceptoFacturaRepository : IPaginadoRepository<ConceptoFactura, ListadoRequest>
{
    Task AddAsync(ConceptoFactura concepto);
    Task<ConceptoFactura?> GetByIdAsync(int id);
    Task<List<ConceptoFactura>> GetAllAsync();
    Task<bool> ExisteAsync(string descripcion, int? idExcluido);
    Task UpdateAsync(ConceptoFactura concepto);
}
