using Itinera.Application.Common.Paginacion;
using Itinera.Application.Facturacion.Interfaces;
using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public sealed class FacturaRepository(AppDbContext context)
    : PaginadoRepository<Factura, ListadoComercialRequest>, IFacturaRepository
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

    protected override IQueryable<Factura> Filtrar(ListadoComercialRequest filtro)
    {
        var consulta = ConRelaciones().AsNoTracking();

        if (filtro.EstadoIds.Count > 0)
            consulta = consulta.Where(factura => filtro.EstadoIds.Contains(factura.EstadoFacturaId));

        if (filtro.ClienteId is int clienteId)
            consulta = consulta.Where(factura => factura.Propuesta.ClienteId == clienteId);

        if (filtro.Desde?.UtcDateTime is DateTime desde)
            consulta = consulta.Where(factura => factura.FechaEmision >= desde);

        if (filtro.Hasta?.UtcDateTime is DateTime hasta)
            consulta = consulta.Where(factura => factura.FechaEmision < hasta);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(factura =>
                AppDbContext.SinTildes(factura.Numero).Contains(texto)
                || ("propuesta #" + factura.PropuestaId).Contains(texto)
                || AppDbContext.SinTildes(factura.Propuesta.Cliente.Nombre + " " + factura.Propuesta.Cliente.Apellido).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Factura> Ordenar(
        IQueryable<Factura> consulta,
        ListadoComercialRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "numero" => consulta.OrdenarPor(factura => factura.Numero, descendente),
            "cliente" => consulta.OrdenarPor(
                factura => factura.Propuesta.Cliente.Nombre + " " + factura.Propuesta.Cliente.Apellido,
                descendente),
            "emision" => consulta.OrdenarPor(factura => factura.FechaEmision, descendente),
            "vencimiento" => consulta.OrdenarPor(factura => factura.FechaVencimiento, descendente),
            "total" => consulta.OrdenarPor(factura => factura.ImporteTotal, descendente),
            "saldo" => consulta.OrdenarPor(factura => factura.SaldoPendiente, descendente),
            "estado" => consulta.OrdenarPor(factura => factura.EstadoFacturaId, descendente),
            _ => consulta.OrderByDescending(factura => factura.FechaEmision)
        };

        return ordenada.ThenByDescending(factura => factura.Id);
    }

    public Task<List<Factura>> GetAllAsync()
        => ConRelaciones()
            .AsNoTracking()
            .OrderByDescending(factura => factura.FechaEmision)
            .ToListAsync();

    public Task<bool> ExisteActivaPorPropuestaAsync(int propuestaId)
        => _context.Facturas.AnyAsync(factura =>
            factura.PropuestaId == propuestaId
            && factura.EstadoFacturaId != (int)Itinera.Domain.Common.EstadoFactura.Anulada);

    public async Task<int> GetSiguienteNumeroAsync(int empresaId)
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
            .IgnoreQueryFilters([
                AppDbContext.FiltroNoEliminada,
                AppDbContext.FiltroPersonaActiva,
                AppDbContext.FiltroEmpresaActiva
            ]);
}
