using Itinera.Application.Ciudades.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class CiudadRepository(AppDbContext context) : ICiudadRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Ciudad ciudad)
    {
        _context.Ciudades.Add(ciudad);
        return _context.SaveChangesAsync();
    }

    public Task<Ciudad?> GetByIdAsync(int id)
        => _context.Ciudades
            .Include(ciudad => ciudad.Pais)
            .FirstOrDefaultAsync(ciudad => ciudad.Id == id);

    public Task<List<Ciudad>> GetAllAsync()
        => _context.Ciudades
            .Include(ciudad => ciudad.Pais)
            .ToListAsync();

    public Task UpdateAsync(Ciudad ciudad)
    {
        _context.Ciudades.Update(ciudad);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Ciudad ciudad)
    {
        _context.Ciudades.Update(ciudad);
        return _context.SaveChangesAsync();
    }
}
