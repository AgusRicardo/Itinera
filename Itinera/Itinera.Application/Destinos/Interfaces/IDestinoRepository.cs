using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Destinos.Interfaces;

public interface IDestinoRepository : IPaginadoRepository<Destino, ListadoRequest>
{
    Task AddAsync(Destino destino);
    Task<Destino?> GetByIdAsync(int id);
    Task<List<Destino>> GetAllAsync();
    Task<bool> ExisteAsync(int ciudadId, int? idExcluido);
    Task<List<Destino>> GetByIdsConActividadesAsync(IEnumerable<int> ids);
    Task UpdateAsync(Destino destino);
    Task DeleteAsync(Destino destino);
}
