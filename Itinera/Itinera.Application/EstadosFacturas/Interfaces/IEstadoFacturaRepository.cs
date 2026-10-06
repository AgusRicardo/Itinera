using Itinera.Domain.Common;

namespace Itinera.Application.EstadosFacturas.Interfaces;

public interface IEstadoFacturaRepository
{
    Task<List<EstadoFacturaCatalogo>> GetAllAsync();
    Task<EstadoFacturaCatalogo?> GetByIdAsync(int id);
}
