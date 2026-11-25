using Itinera.Application.Common.Paginacion;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class DestinoRepository(AppDbContext context)
    : PaginadoRepository<Destino, ListadoRequest>, IDestinoRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Destino destino)
    {
        _context.Destinos.Add(destino);
        return _context.SaveChangesAsync();
    }

    public Task<Destino?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(destino => destino.Id == id);

    protected override IQueryable<Destino> Filtrar(ListadoRequest filtro)
    {
        IQueryable<Destino> consulta = ConRelaciones();

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(destino =>
                AppDbContext.SinTildes(destino.Ciudad.Nombre + ", " + destino.Ciudad.Pais.Nombre).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Destino> Ordenar(
        IQueryable<Destino> consulta,
        ListadoRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "ciudad" => consulta.OrdenarPor(destino => destino.Ciudad.Nombre, descendente),
            "pais" => consulta.OrdenarPor(destino => destino.Ciudad.Pais.Nombre, descendente),
            _ => consulta.OrderBy(destino => destino.Ciudad.Pais.Nombre)
        };

        return ordenada
            .ThenBy(destino => destino.Ciudad.Pais.Nombre)
            .ThenBy(destino => destino.Ciudad.Nombre)
            .ThenBy(destino => destino.Id);
    }

    public Task<bool> ExisteAsync(int ciudadId, int? idExcluido)
        => _context.Destinos.AnyAsync(destino =>
            destino.CiudadId == ciudadId
            && destino.Id != idExcluido);

    public Task<List<Destino>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    public Task<List<Destino>> GetByIdsConActividadesAsync(IEnumerable<int> ids)
    {
        var listaIds = ids.ToList();

        return _context.Destinos
            .Include(destino => destino.Actividades)
            .Include(destino => destino.Ciudad)
                .ThenInclude(ciudad => ciudad.Pais)
            .Where(destino => listaIds.Contains(destino.Id))
            .ToListAsync();
    }

    private IQueryable<Destino> ConRelaciones()
        => _context.Destinos
            .Include(destino => destino.Ciudad)
                .ThenInclude(ciudad => ciudad.Pais)
            .Where(destino => destino.Ciudad.Pais.Activo);

    public Task UpdateAsync(Destino destino)
    {
        _context.Destinos.Update(destino);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Destino destino)
    {
        _context.Destinos.Update(destino);
        return _context.SaveChangesAsync();
    }
}
