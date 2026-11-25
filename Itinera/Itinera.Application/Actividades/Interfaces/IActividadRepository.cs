using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Actividades.Interfaces;

public interface IActividadRepository : IPaginadoRepository<Actividad, ListarActividadesRequest>
{
    Task AddAsync(Actividad actividad);
    Task<Actividad?> GetByIdAsync(int id);
    Task<Actividad?> GetByIdConInactivasAsync(int id);
    Task<List<Actividad>> GetAllAsync();
    Task UpdateAsync(Actividad actividad);
    Task DeleteAsync(Actividad actividad);
}
