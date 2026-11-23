using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Actividades.Interfaces;

public interface IActividadRepository
{
    Task AddAsync(Actividad actividad);
    Task<Actividad?> GetByIdAsync(int id);
    Task<List<Actividad>> GetAllAsync();
    Task<Pagina<Actividad>> GetPaginaAsync(PaginacionRequest paginacion);
    Task UpdateAsync(Actividad actividad);
    Task DeleteAsync(Actividad actividad);
}
