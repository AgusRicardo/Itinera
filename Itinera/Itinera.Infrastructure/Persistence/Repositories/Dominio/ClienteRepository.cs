using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class ClienteRepository(AppDbContext context, ITenantContext tenantContext)
    : PaginadoRepository<Cliente, ListadoActivableRequest>, IClienteRepository
{
    private readonly AppDbContext _context = context;
    private int? EmpresaId => tenantContext.EmpresaId;
    private bool EsAdministradorPlataforma => tenantContext.EsAdministradorPlataforma;

    public Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        return _context.SaveChangesAsync();
    }

    public Task<Cliente?> GetByIdAsync(int id)
        => DeLaEmpresa().FirstOrDefaultAsync(cliente => cliente.Id == id);

    public Task<Cliente?> GetByIdConInactivosAsync(int id)
        => ConInactivos().FirstOrDefaultAsync(cliente => cliente.Id == id);

    public Task<List<Cliente>> GetAllAsync()
        => DeLaEmpresa().ToListAsync();

    protected override IQueryable<Cliente> Filtrar(ListadoActivableRequest filtro)
    {
        IQueryable<Cliente> consulta = ConInactivos().Include(cliente => cliente.Propuestas);

        if (filtro.Activo is bool activo)
            consulta = consulta.Where(cliente => cliente.Activo == activo);

        if (filtro.TextoNormalizado is string texto)
            consulta = consulta.Where(cliente =>
                AppDbContext.SinTildes(cliente.Nombre + " " + cliente.Apellido).Contains(texto)
                || AppDbContext.SinTildes(cliente.Email).Contains(texto)
                || AppDbContext.SinTildes(cliente.Telefono).Contains(texto));

        return consulta;
    }

    protected override IOrderedQueryable<Cliente> Ordenar(
        IQueryable<Cliente> consulta,
        ListadoActivableRequest filtro)
    {
        var descendente = filtro.Descendente;
        var ordenada = filtro.Orden switch
        {
            "contacto" => consulta.OrdenarPor(cliente => cliente.Email, descendente),
            "propuestas" => consulta.OrdenarPor(cliente => cliente.Propuestas.Count(), descendente),
            "ultimaGestion" => consulta.OrdenarPor(
                cliente => cliente.Propuestas.Max(propuesta =>
                    (DateTime?)(propuesta.FechaModificacion ?? propuesta.FechaCreacion)) ?? DateTime.UnixEpoch,
                descendente),
            "estado" => consulta.OrdenarPor(cliente => cliente.Activo, descendente),
            _ => consulta.OrdenarPor(cliente => cliente.Nombre, descendente)
                .ThenBy(cliente => cliente.Apellido)
        };

        return ordenada.ThenBy(cliente => cliente.Id);
    }

    public async Task<ResumenClientesResponse> GetResumenAsync()
    {
        var hoy = DateTime.UtcNow;
        var inicioDeMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return new ResumenClientesResponse
        {
            Total = await ConInactivos().CountAsync(),
            Activos = await ConInactivos().CountAsync(cliente => cliente.Activo),
            ConPropuestas = await ConInactivos().CountAsync(cliente => cliente.Propuestas.Any()),
            NuevosEsteMes = await ConInactivos().CountAsync(cliente => cliente.FechaRegistracion >= inicioDeMes)
        };
    }

    private IQueryable<Cliente> ConInactivos()
        => DeLaEmpresa().IgnoreQueryFilters([AppDbContext.FiltroPersonaActiva]);

    private IQueryable<Cliente> DeLaEmpresa()
        => _context.Clientes
            .Where(cliente => EsAdministradorPlataforma
                || (EmpresaId != null && cliente.Empresas.Any(ec => ec.EmpresaId == EmpresaId)));

    public Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        return _context.SaveChangesAsync();
    }
}
