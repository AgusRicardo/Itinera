using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Paises.Interfaces;

public interface IPaisRepository : IPaginadoRepository<Pais, ListadoRequest>
{
    Task AddAsync(Pais pais);
    Task<Pais?> GetByIdAsync(int id);
    Task<List<Pais>> GetAllAsync();
    Task<bool> ExisteAsync(string nombre, int? idExcluido);
    Task UpdateAsync(Pais pais);
    Task DeleteAsync(Pais pais);
}
