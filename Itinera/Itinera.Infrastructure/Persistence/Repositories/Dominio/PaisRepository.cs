using Itinera.Application.Paises.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class PaisRepository(AppDbContext context) : IPaisRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Pais pais)
    {
        _context.Paises.Add(pais);
        return _context.SaveChangesAsync();
    }

    public Task<Pais?> GetByIdAsync(int id)
        => _context.Paises.FindAsync(id).AsTask();

    public Task<List<Pais>> GetAllAsync()
        => _context.Paises.ToListAsync();

    public Task UpdateAsync(Pais pais)
    {
        _context.Paises.Update(pais);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Pais pais)
    {
        _context.Paises.Update(pais);
        return _context.SaveChangesAsync();
    }
}
