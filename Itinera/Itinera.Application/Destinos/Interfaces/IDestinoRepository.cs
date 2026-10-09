using Itinera.Domain.Propuestas;

namespace Itinera.Application.Destinos.Interfaces;

public interface IDestinoRepository
{
    Task AddAsync(Destino destino);
    Task<Destino?> GetByIdAsync(int id);
    Task<List<Destino>> GetAllAsync();
    Task<List<Destino>> ObtenerPorIdsConActividadesAsync(IEnumerable<int> ids);
    Task UpdateAsync(Destino destino);
    Task DeleteAsync(Destino destino);
}
