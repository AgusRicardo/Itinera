using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Paises.Interfaces;

public interface IPaisRepository
{
    Task AddAsync(Pais pais);
    Task<Pais?> GetByIdAsync(int id);
    Task<List<Pais>> GetAllAsync();
    Task<Pagina<Pais>> GetPaginaAsync(PaginacionRequest paginacion);
    Task UpdateAsync(Pais pais);
    Task DeleteAsync(Pais pais);
}
