using Itinera.Application.Common.Paginacion;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Domain.Empresa;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EmpresaRepository(AppDbContext context)
    : PaginadoRepository<Empresa, ListadoActivableRequest>, IEmpresaRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Empresa empresa)
    {
        _context.Empresas.Add(empresa);
        return _context.SaveChangesAsync();
    }

    public Task<Empresa?> GetByIdAsync(int id)
        => _context.Empresas.FindAsync(id).AsTask();

    public Task<List<Empresa>> GetAllConInactivasAsync()
        => ConInactivas().OrderBy(empresa => empresa.RazonSocial).ToListAsync();

    public Task<Empresa?> GetByIdConInactivasAsync(int id)
        => ConInactivas().FirstOrDefaultAsync(empresa => empresa.Id == id);

    protected override IQueryable<Empresa> Filtrar(ListadoActivableRequest filtro)
    {
        var consulta = ConInactivas();

        if (filtro.Activo is bool activo)
            consulta = consulta.Where(empresa => empresa.Activo == activo);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(empresa =>
                AppDbContext.SinTildes(empresa.RazonSocial).Contains(texto)
                || AppDbContext.SinTildes(empresa.CUIT).Contains(texto)
                || AppDbContext.SinTildes(empresa.Telefono).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Empresa> Ordenar(
        IQueryable<Empresa> consulta,
        ListadoActivableRequest filtro)
    {
        var ordenada = filtro.Orden switch
        {
            "cuit" => consulta.OrdenarPor(empresa => empresa.CUIT, filtro.Descendente),
            "telefono" => consulta.OrdenarPor(empresa => empresa.Telefono, filtro.Descendente),
            "estado" => consulta.OrdenarPor(empresa => empresa.Activo, filtro.Descendente),
            _ => consulta.OrdenarPor(empresa => empresa.RazonSocial, filtro.Descendente)
        };

        return ordenada.ThenBy(empresa => empresa.Id);
    }

    private IQueryable<Empresa> ConInactivas()
        => _context.Empresas.IgnoreQueryFilters([AppDbContext.FiltroEmpresaActiva]);

    public Task UpdateAsync(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Empresa empresa)
    {
        _context.Empresas.Update(empresa);
        return _context.SaveChangesAsync();
    }
}
