using Itinera.Application.Actividades.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class ActividadRepository(AppDbContext context) : IActividadRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Actividad actividad)
    {
        _context.Actividades.Add(actividad);
        return _context.SaveChangesAsync();
    }

    public Task<Actividad?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(actividad => actividad.Id == id);

    public Task<List<Actividad>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    private IQueryable<Actividad> ConRelaciones()
        => _context.Actividades
            .Include(actividad => actividad.Destino)
                .ThenInclude(destino => destino.Ciudad)
                    .ThenInclude(ciudad => ciudad.Pais);

    public Task UpdateAsync(Actividad actividad)
    {
        _context.Actividades.Update(actividad);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Actividad actividad)
    {
        _context.Actividades.Update(actividad);
        return _context.SaveChangesAsync();
    }
}
