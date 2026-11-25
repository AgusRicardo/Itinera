using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Actividades.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class ActividadRepository(AppDbContext context)
    : PaginadoRepository<Actividad, ListarActividadesRequest>, IActividadRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Actividad actividad)
    {
        _context.Actividades.Add(actividad);
        return _context.SaveChangesAsync();
    }

    public Task<Actividad?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(actividad => actividad.Id == id);

    public Task<Actividad?> GetByIdConInactivasAsync(int id)
        => _context.Actividades
            .IgnoreQueryFilters([AppDbContext.FiltroCatalogoActivo])
            .FirstOrDefaultAsync(actividad => actividad.Id == id);

    protected override IQueryable<Actividad> Filtrar(ListarActividadesRequest filtro)
    {
        IQueryable<Actividad> consulta = ConRelaciones();

        if (filtro.DestinoId is int destinoId)
            consulta = consulta.Where(actividad => actividad.DestinoId == destinoId);

        if (filtro.GeneradoPorIA is bool generadoPorIA)
            consulta = consulta.Where(actividad => actividad.GeneradoPorIA == generadoPorIA);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(actividad =>
                AppDbContext.SinTildes(actividad.Nombre).Contains(texto)
                || AppDbContext.SinTildes(actividad.Descripcion).Contains(texto)
                || AppDbContext.SinTildes(actividad.Destino.Ciudad.Nombre + ", " + actividad.Destino.Ciudad.Pais.Nombre).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Actividad> Ordenar(
        IQueryable<Actividad> consulta,
        ListarActividadesRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "nombre" => consulta.OrdenarPor(actividad => actividad.Nombre, descendente),
            "destino" => consulta.OrdenarPor(actividad => actividad.Destino.Ciudad.Nombre, descendente),
            "costo" => consulta.OrdenarPor(actividad => actividad.CostoBase, descendente),
            _ => consulta.OrderBy(actividad => actividad.Destino.Ciudad.Pais.Nombre)
        };

        return ordenada
            .ThenBy(actividad => actividad.Destino.Ciudad.Pais.Nombre)
            .ThenBy(actividad => actividad.Destino.Ciudad.Nombre)
            .ThenBy(actividad => actividad.Nombre)
            .ThenBy(actividad => actividad.Id);
    }

    public Task<List<Actividad>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    private IQueryable<Actividad> ConRelaciones()
        => _context.Actividades
            .Include(actividad => actividad.Destino)
                .ThenInclude(destino => destino.Ciudad)
                    .ThenInclude(ciudad => ciudad.Pais)
            .Where(actividad => actividad.Destino.Ciudad.Pais.Activo);

    public Task UpdateAsync(Actividad actividad)
    {
        _context.Actividades.Update(actividad);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Actividad actividad)
    {
        _context.Actividades.Update(actividad);
        return _context.SaveChangesAsync();
    }
}
