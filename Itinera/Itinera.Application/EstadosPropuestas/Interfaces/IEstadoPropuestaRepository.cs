using Itinera.Domain.Common;

namespace Itinera.Application.EstadosPropuestas.Interfaces;

public interface IEstadoPropuestaRepository
{
    Task<List<EstadoPropuestaCatalogo>> GetAllAsync();
    Task<EstadoPropuestaCatalogo?> GetByIdAsync(int id);
}
