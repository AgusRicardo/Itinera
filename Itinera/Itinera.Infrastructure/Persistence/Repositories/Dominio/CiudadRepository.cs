using Itinera.Application.Common.Paginacion;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class CiudadRepository(AppDbContext context)
    : PaginadoRepository<Ciudad, ListadoRequest>, ICiudadRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Ciudad ciudad)
    {
        _context.Ciudades.Add(ciudad);
        return _context.SaveChangesAsync();
    }

    public Task<Ciudad?> GetByIdAsync(int id)
        => ConRelaciones().FirstOrDefaultAsync(ciudad => ciudad.Id == id);

    protected override IQueryable<Ciudad> Filtrar(ListadoRequest filtro)
    {
        IQueryable<Ciudad> consulta = ConRelaciones();

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(ciudad =>
                AppDbContext.SinTildes(ciudad.Nombre + ", " + ciudad.Pais.Nombre).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Ciudad> Ordenar(
        IQueryable<Ciudad> consulta,
        ListadoRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "nombre" => consulta.OrdenarPor(ciudad => ciudad.Nombre, descendente),
            "pais" => consulta.OrdenarPor(ciudad => ciudad.Pais.Nombre, descendente),
            _ => consulta.OrderBy(ciudad => ciudad.Pais.Nombre)
        };

        return ordenada
            .ThenBy(ciudad => ciudad.Pais.Nombre)
            .ThenBy(ciudad => ciudad.Nombre)
            .ThenBy(ciudad => ciudad.Id);
    }

    public Task<bool> ExisteAsync(string nombre, int paisId, int? idExcluido)
        => _context.Ciudades.AnyAsync(ciudad =>
            ciudad.Nombre.Trim().ToLower() == nombre.ToLower()
            && ciudad.PaisId == paisId
            && ciudad.Id != idExcluido);

    public Task<List<Ciudad>> GetAllAsync()
        => ConRelaciones().ToListAsync();

    private IQueryable<Ciudad> ConRelaciones()
        => _context.Ciudades
            .Include(ciudad => ciudad.Pais)
            .Where(ciudad => ciudad.Pais.Activo);

    public Task UpdateAsync(Ciudad ciudad)
    {
        _context.Ciudades.Update(ciudad);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Ciudad ciudad)
    {
        _context.Ciudades.Update(ciudad);
        return _context.SaveChangesAsync();
    }
}
