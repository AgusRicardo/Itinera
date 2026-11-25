using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class PropuestaRepository(AppDbContext context)
    : PaginadoRepository<Propuesta, ListadoComercialRequest>, IPropuestaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Propuesta propuesta)
    {
        _context.Propuestas.Add(propuesta);
        return _context.SaveChangesAsync();
    }

    public Task<Propuesta?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(propuesta => propuesta.Id == id);

    public Task<List<Propuesta>> GetAllAsync()
        => ConRelaciones()
            .OrderByDescending(propuesta => propuesta.FechaCreacion)
            .ToListAsync();

    protected override IQueryable<Propuesta> Filtrar(ListadoComercialRequest filtro)
    {
        var consulta = ConRelaciones();

        if (filtro.EstadoIds.Count > 0)
            consulta = consulta.Where(propuesta => filtro.EstadoIds.Contains(propuesta.EstadoPropuestaId));

        if (filtro.ClienteId is int clienteId)
            consulta = consulta.Where(propuesta => propuesta.ClienteId == clienteId);

        if (filtro.Desde?.UtcDateTime is DateTime desde)
            consulta = consulta.Where(propuesta => propuesta.FechaCreacion >= desde);

        if (filtro.Hasta?.UtcDateTime is DateTime hasta)
            consulta = consulta.Where(propuesta => propuesta.FechaCreacion < hasta);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(propuesta =>
                AppDbContext.SinTildes(propuesta.Cliente.Nombre + " " + propuesta.Cliente.Apellido).Contains(texto)
                || propuesta.Destinos.Any(item =>
                    AppDbContext.SinTildes(item.Destino.Ciudad.Nombre + ", " + item.Destino.Ciudad.Pais.Nombre).Contains(texto)));

        return consulta;
    }

    protected override IOrderedQueryable<Propuesta> Ordenar(
        IQueryable<Propuesta> consulta,
        ListadoComercialRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "propuesta" => consulta.OrdenarPor(propuesta => propuesta.Id, descendente),
            "fechaCreacion" => consulta.OrdenarPor(propuesta => propuesta.FechaCreacion, descendente),
            "cliente" => consulta.OrdenarPor(
                propuesta => propuesta.Cliente.Nombre + " " + propuesta.Cliente.Apellido, descendente),
            "destino" => consulta.OrdenarPor(
                propuesta => propuesta.Destinos
                    .OrderBy(item => item.Orden)
                    .Select(item => item.Destino.Ciudad.Nombre)
                    .FirstOrDefault(),
                descendente),
            "fechas" => consulta.OrdenarPor(propuesta => propuesta.FechaInicio, descendente),
            "presupuesto" => consulta.OrdenarPor(propuesta => propuesta.Presupuesto, descendente),
            "estado" => consulta.OrdenarPor(propuesta => propuesta.EstadoPropuestaId, descendente),
            _ => consulta.OrderByDescending(propuesta => propuesta.FechaCreacion)
        };

        return ordenada.ThenByDescending(propuesta => propuesta.Id);
    }

    public async Task<ResumenPropuestasResponse> GetResumenAsync()
    {
        var cantidades = await _context.Propuestas
            .GroupBy(propuesta => propuesta.EstadoPropuestaId)
            .Select(grupo => new { EstadoId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(grupo => grupo.EstadoId, grupo => grupo.Cantidad);

        return new ResumenPropuestasResponse
        {
            Total = cantidades.Values.Sum(),
            Borradores = cantidades.GetValueOrDefault((int)EstadoPropuesta.Borrador),
            Presentadas = cantidades.GetValueOrDefault((int)EstadoPropuesta.Presentada),
            Aceptadas = cantidades.GetValueOrDefault((int)EstadoPropuesta.Aceptada)
        };
    }

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
            .Include(propuesta => propuesta.Etiquetas)
                .ThenInclude(item => item.Etiqueta)
            .Include(propuesta => propuesta.Destinos)
                .ThenInclude(item => item.Destino)
                    .ThenInclude(destino => destino.Ciudad)
                        .ThenInclude(ciudad => ciudad.Pais)
            .Include(propuesta => propuesta.Actividades)
                .ThenInclude(item => item.Actividad)
            .Include(propuesta => propuesta.Itinerarios)
                .ThenInclude(itinerario => itinerario.Destino)
            .Include(propuesta => propuesta.Itinerarios)
                .ThenInclude(itinerario => itinerario.Actividades)
                    .ThenInclude(item => item.Actividad)
            .IgnoreQueryFilters([AppDbContext.FiltroPersonaActiva, AppDbContext.FiltroCatalogoActivo])
            .AsSplitQuery();
}
