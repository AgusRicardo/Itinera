using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EstadoPropuestaRepository(AppDbContext context) : IEstadoPropuestaRepository
{
    private readonly AppDbContext _context = context;

    public Task<List<EstadoPropuestaCatalogo>> GetAllAsync()
        => _context.EstadosPropuesta
            .AsNoTracking()
            .OrderBy(estado => estado.Id)
            .ToListAsync();

    public Task<EstadoPropuestaCatalogo?> GetByIdAsync(int id)
        => _context.EstadosPropuesta
            .AsNoTracking()
            .FirstOrDefaultAsync(estado => estado.Id == id);
}
