using Itinera.Domain.Propuestas;

namespace Itinera.Application.Actividades.Interfaces;

public interface IActividadRepository
{
    Task AddAsync(Actividad actividad);
    Task<Actividad?> GetByIdAsync(int id);
    Task<List<Actividad>> GetAllAsync();
    Task UpdateAsync(Actividad actividad);
    Task DeleteAsync(Actividad actividad);
}
