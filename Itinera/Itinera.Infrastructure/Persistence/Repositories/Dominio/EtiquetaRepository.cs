using Itinera.Application.Common.Interfaces;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Domain.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public sealed class EtiquetaRepository(
    AppDbContext context,
    ITenantContext tenantContext) : IEtiquetaRepository
{
    private readonly AppDbContext _context = context;
    private readonly ITenantContext _tenantContext = tenantContext;

    public Task AddAsync(Etiqueta etiqueta)
    {
        _context.Etiquetas.Add(etiqueta);
        return _context.SaveChangesAsync();
    }

    public Task<Etiqueta?> GetByIdAsync(int id)
        => _context.Etiquetas.FirstOrDefaultAsync(etiqueta => etiqueta.Id == id);

    public Task<List<Etiqueta>> GetAllAsync()
        => _context.Etiquetas
            .AsNoTracking()
            .Where(etiqueta => etiqueta.Activo)
            .OrderBy(etiqueta => etiqueta.Nombre)
            .ToListAsync();

    public Task<List<Etiqueta>> GetByIdsAsync(IEnumerable<int> ids)
        => _context.Etiquetas
            .Where(etiqueta => ids.Contains(etiqueta.Id) && etiqueta.Activo)
            .ToListAsync();

    public Task UpdateAsync(Etiqueta etiqueta)
        => _context.SaveChangesAsync();
}
