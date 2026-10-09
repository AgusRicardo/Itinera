using Itinera.Application.Destinos.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class DestinoRepository(AppDbContext context) : IDestinoRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Destino destino)
    {
        _context.Destinos.Add(destino);
        return _context.SaveChangesAsync();
    }

    public Task<Destino?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(destino => destino.Id == id);

    public Task<List<Destino>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    public Task<List<Destino>> ObtenerPorIdsConActividadesAsync(IEnumerable<int> ids)
    {
        var listaIds = ids.ToList();

        return _context.Destinos
            .Include(destino => destino.Actividades)
            .Include(destino => destino.Ciudad)
                .ThenInclude(ciudad => ciudad.Pais)
            .Where(destino => listaIds.Contains(destino.Id))
            .ToListAsync();
    }

    private IQueryable<Destino> ConRelaciones()
        => _context.Destinos
            .Include(destino => destino.Ciudad)
                .ThenInclude(ciudad => ciudad.Pais);

    public Task UpdateAsync(Destino destino)
    {
        _context.Destinos.Update(destino);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Destino destino)
    {
        _context.Destinos.Update(destino);
        return _context.SaveChangesAsync();
    }
}
