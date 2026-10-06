using Itinera.Domain.Propuestas;

namespace Itinera.Application.Ciudades.Interfaces;

public interface ICiudadRepository
{
    Task AddAsync(Ciudad ciudad);
    Task<Ciudad?> GetByIdAsync(int id);
    Task<List<Ciudad>> GetAllAsync();
    Task UpdateAsync(Ciudad ciudad);
    Task DeleteAsync(Ciudad ciudad);
}
