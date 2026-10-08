using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class PropuestaRepository(AppDbContext context) : IPropuestaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Propuesta propuesta)
    {
        _context.Propuestas.Add(propuesta);
        return _context.SaveChangesAsync();
    }

    public Task<Propuesta?> GetByIdAsync(int id)
        => _context.Propuestas
            .Include(propuesta => propuesta.Cliente)
            .Include(propuesta => propuesta.Empleado)
            .Include(propuesta => propuesta.Itinerario)
            .FirstOrDefaultAsync(propuesta => propuesta.Id == id);
}
