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
        => ConRelaciones().FirstOrDefaultAsync(propuesta => propuesta.Id == id);

    public Task<List<Propuesta>> GetByClienteAsync(int clienteId)
        => ConRelaciones()
            .Where(propuesta => propuesta.ClienteId == clienteId)
            .OrderByDescending(propuesta => propuesta.FechaCreacion)
            .ToListAsync();

    public Task UpdateAsync(Propuesta propuesta)
    {
        _context.Entry(propuesta).State = EntityState.Modified;
        return _context.SaveChangesAsync();
    }

    private IQueryable<Propuesta> ConRelaciones()
        => _context.Propuestas
            .Include(propuesta => propuesta.Cliente)
            .Include(propuesta => propuesta.Empleado)
            .Include(propuesta => propuesta.Itinerario)
                .ThenInclude(itinerario => itinerario.Destinos)
                    .ThenInclude(destinoItinerario => destinoItinerario.Destino)
                        .ThenInclude(destino => destino.Ciudad)
                            .ThenInclude(ciudad => ciudad.Pais)
            .Include(propuesta => propuesta.Itinerario)
                .ThenInclude(itinerario => itinerario.Destinos)
                    .ThenInclude(destinoItinerario => destinoItinerario.ActividadDestinoItinerarios)
                        .ThenInclude(actividad => actividad.Actividad);
}
