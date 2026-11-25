using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Facturacion;

namespace Itinera.Application.Facturacion.Interfaces;

public interface IFacturaRepository : IPaginadoRepository<Factura, ListadoComercialRequest>
{
    Task AddAsync(Factura factura);
    Task<Factura?> GetByIdAsync(int id);
    Task<List<Factura>> GetAllAsync();
    Task<bool> ExisteActivaPorPropuestaAsync(int propuestaId);
    Task<int> GetSiguienteNumeroAsync(int empresaId);
    Task UpdateAsync(Factura factura);
}
