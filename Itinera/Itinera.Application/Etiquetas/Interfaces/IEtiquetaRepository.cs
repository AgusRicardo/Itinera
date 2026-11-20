using Itinera.Domain.Catalogos;

namespace Itinera.Application.Etiquetas.Interfaces;

public interface IEtiquetaRepository
{
    Task AddAsync(Etiqueta etiqueta);
    Task<Etiqueta?> GetByIdAsync(int id);
    Task<List<Etiqueta>> GetAllAsync();
    Task<List<Etiqueta>> GetByIdsAsync(IEnumerable<int> ids);
    Task UpdateAsync(Etiqueta etiqueta);
}
