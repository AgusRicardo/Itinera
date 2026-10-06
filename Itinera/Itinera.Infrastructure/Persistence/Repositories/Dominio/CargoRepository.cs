using Itinera.Application.Cargos.Interfaces;
using Itinera.Domain.Empresa;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class CargoRepository(AppDbContext context) : ICargoRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Cargo cargo)
    {
        _context.Cargos.Add(cargo);
        return _context.SaveChangesAsync();
    }

    public Task<Cargo?> GetByIdAsync(int id)
        => _context.Cargos.FindAsync(id).AsTask();

    public Task<List<Cargo>> GetAllAsync()
        => _context.Cargos.ToListAsync();

    public Task UpdateAsync(Cargo cargo)
    {
        _context.Cargos.Update(cargo);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Cargo cargo)
    {
        _context.Cargos.Update(cargo);
        return _context.SaveChangesAsync();
    }
}
