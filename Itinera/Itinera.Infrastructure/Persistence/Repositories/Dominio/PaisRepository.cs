using Itinera.Application.Common.Paginacion;
using Itinera.Application.Paises.Interfaces;
using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class PaisRepository(AppDbContext context)
    : PaginadoRepository<Pais, ListadoRequest>, IPaisRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Pais pais)
    {
        _context.Paises.Add(pais);
        return _context.SaveChangesAsync();
    }

    public Task<Pais?> GetByIdAsync(int id)
        => _context.Paises.FindAsync(id).AsTask();

    protected override IQueryable<Pais> Filtrar(ListadoRequest filtro)
    {
        IQueryable<Pais> consulta = _context.Paises;

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(pais => AppDbContext.SinTildes(pais.Nombre).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Pais> Ordenar(
        IQueryable<Pais> consulta,
        ListadoRequest filtro)
        => consulta.OrdenarPor(pais => pais.Nombre, filtro.Descendente)
            .ThenBy(pais => pais.Id);

    public Task<bool> ExisteAsync(string nombre, int? idExcluido)
        => _context.Paises.AnyAsync(pais =>
            pais.Nombre.Trim().ToLower() == nombre.ToLower()
            && pais.Id != idExcluido);

    public Task<List<Pais>> GetAllAsync()
        => _context.Paises.ToListAsync();

    public Task UpdateAsync(Pais pais)
    {
        _context.Paises.Update(pais);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Pais pais)
    {
        _context.Paises.Update(pais);
        return _context.SaveChangesAsync();
    }
}
