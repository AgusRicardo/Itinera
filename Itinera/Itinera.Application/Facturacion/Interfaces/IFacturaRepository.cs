using Itinera.Domain.Facturacion;

namespace Itinera.Application.Facturacion.Interfaces;

public interface IFacturaRepository
{
    Task AddAsync(Factura factura);
    Task<Factura?> GetByIdAsync(int id);
    Task<List<Factura>> GetAllAsync();
    Task<bool> ExisteActivaPorPropuestaAsync(int propuestaId);
    Task<int> ObtenerSiguienteNumeroAsync(int empresaId);
    Task UpdateAsync(Factura factura);
}
