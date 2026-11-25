using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Ciudades.Interfaces;

public interface ICiudadRepository : IPaginadoRepository<Ciudad, ListadoRequest>
{
    Task AddAsync(Ciudad ciudad);
    Task<Ciudad?> GetByIdAsync(int id);
    Task<List<Ciudad>> GetAllAsync();
    Task<bool> ExisteAsync(string nombre, int paisId, int? idExcluido);
    Task UpdateAsync(Ciudad ciudad);
    Task DeleteAsync(Ciudad ciudad);
}
