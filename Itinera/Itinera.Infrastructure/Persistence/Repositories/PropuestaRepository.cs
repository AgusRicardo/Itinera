using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Propuestas;

namespace Itinera.Infrastructure.Persistence.Repositories;

public class PropuestaRepository(AppDbContext context) : IPropuestaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Propuesta propuesta)
    {
        throw new NotImplementedException();
    }

    public async Task<Propuesta?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
