using Itinera.Application.Common.Paginacion;
using Itinera.Application.Conceptos.Interfaces;
using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public sealed class ConceptoFacturaRepository(AppDbContext context)
    : PaginadoRepository<ConceptoFactura, ListadoRequest>, IConceptoFacturaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(ConceptoFactura concepto)
    {
        _context.ConceptosFactura.Add(concepto);
        return _context.SaveChangesAsync();
    }

    public Task<ConceptoFactura?> GetByIdAsync(int id)
        => Activos().FirstOrDefaultAsync(concepto => concepto.Id == id);

    public Task<List<ConceptoFactura>> GetAllAsync()
        => Activos().OrderBy(concepto => concepto.Descripcion).ToListAsync();

    public Task<bool> ExisteAsync(string descripcion, int? idExcluido)
        => Activos().AnyAsync(concepto =>
            concepto.Descripcion.ToLower() == descripcion.ToLower()
            && concepto.Id != idExcluido);

    public Task UpdateAsync(ConceptoFactura concepto)
        => _context.SaveChangesAsync();

    protected override IQueryable<ConceptoFactura> Filtrar(ListadoRequest filtro)
        => filtro.TextoNormalizado is string texto
            ? Activos().Where(concepto => AppDbContext.SinTildes(concepto.Descripcion).Contains(texto))
            : Activos();

    protected override IOrderedQueryable<ConceptoFactura> Ordenar(
        IQueryable<ConceptoFactura> consulta,
        ListadoRequest filtro)
    {
        var ordenada = filtro.Orden switch
        {
            "precio" => consulta.OrderBy(concepto => concepto.Porcentaje != null)
                .ThenBy(concepto => concepto.Precio * (filtro.Descendente ? -1 : 1)),
            "porcentaje" => consulta.OrderBy(concepto => concepto.Porcentaje == null)
                .ThenBy(concepto => concepto.Porcentaje * (filtro.Descendente ? -1 : 1)),
            _ => consulta.OrdenarPor(concepto => concepto.Descripcion, filtro.Descendente)
        };

        return ordenada.ThenBy(concepto => concepto.Id);
    }

    private IQueryable<ConceptoFactura> Activos()
        => _context.ConceptosFactura.Where(concepto => concepto.Activo);
}
