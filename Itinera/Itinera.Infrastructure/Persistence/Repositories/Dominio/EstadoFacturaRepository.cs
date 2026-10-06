using Itinera.Application.EstadosFacturas.Interfaces;
using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EstadoFacturaRepository(AppDbContext context) : IEstadoFacturaRepository
{
    private readonly AppDbContext _context = context;

    public Task<List<EstadoFacturaCatalogo>> GetAllAsync()
        => _context.EstadosFactura
            .AsNoTracking()
            .OrderBy(estado => estado.Id)
            .ToListAsync();

    public Task<EstadoFacturaCatalogo?> GetByIdAsync(int id)
        => _context.EstadosFactura
            .AsNoTracking()
            .FirstOrDefaultAsync(estado => estado.Id == id);
}
