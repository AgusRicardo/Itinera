using Itinera.Application.Common.Paginacion;
using Itinera.Application.Facturacion.Interfaces;
using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public sealed class FacturaRepository(AppDbContext context) : IFacturaRepository
{
    private static readonly SemaphoreSlim SemaforoNumeracion = new(1, 1);
    private readonly AppDbContext _context = context;

    public Task AddAsync(Factura factura)
    {
        _context.Facturas.Add(factura);
        return _context.SaveChangesAsync();
    }

    public Task<Factura?> GetByIdAsync(int id)
        => ConRelaciones()
            .FirstOrDefaultAsync(factura => factura.Id == id);

    public Task<Pagina<Factura>> GetPaginaAsync(PaginacionRequest paginacion)
        => ConRelaciones()
            .AsNoTracking()
            .OrderByDescending(factura => factura.FechaEmision)
            .ThenByDescending(factura => factura.Id)
            .PaginarAsync(paginacion);

    public Task<List<Factura>> GetAllAsync()
        => ConRelaciones()
            .AsNoTracking()
            .OrderByDescending(factura => factura.FechaEmision)
            .ToListAsync();

    public Task<bool> ExisteActivaPorPropuestaAsync(int propuestaId)
        => _context.Facturas.AnyAsync(factura =>
            factura.PropuestaId == propuestaId
            && factura.EstadoFacturaId != (int)Itinera.Domain.Common.EstadoFactura.Anulada);

    public async Task<int> ObtenerSiguienteNumeroAsync(int empresaId)
    {
        await SemaforoNumeracion.WaitAsync();
        try
        {
            var numeros = await _context.Facturas
                .Where(factura =>
                    factura.EmpresaId == empresaId
                    && factura.Numero.StartsWith("FAC-"))
                .Select(factura => factura.Numero)
                .ToListAsync();

            var ultimoNumero = numeros
                .Select(numero => int.TryParse(numero[4..], out var valor) ? valor : 0)
                .DefaultIfEmpty()
                .Max();

            return ultimoNumero + 1;
        }
        finally
        {
            SemaforoNumeracion.Release();
        }
    }

    public Task UpdateAsync(Factura factura)
        => _context.SaveChangesAsync();

    private IQueryable<Factura> ConRelaciones()
        => _context.Facturas
            .Include(factura => factura.Propuesta)
                .ThenInclude(propuesta => propuesta.Cliente)
            .Include(factura => factura.Propuesta)
                .ThenInclude(propuesta => propuesta.Empleado)
                    .ThenInclude(empleado => empleado.Empresa)
            .Include(factura => factura.Detalles)
            .Include(factura => factura.Pagos)
            .IgnoreQueryFilters([AppDbContext.FiltroNoEliminada]);
}
