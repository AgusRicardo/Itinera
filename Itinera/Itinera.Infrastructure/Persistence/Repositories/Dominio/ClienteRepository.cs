using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class ClienteRepository(AppDbContext context, ITenantContext tenantContext) : IClienteRepository
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

    public Task<List<Cliente>> GetAllAsync()
        => DeLaEmpresa().ToListAsync();

    public Task<Pagina<Cliente>> GetPaginaAsync(ListarClientesRequest request)
    {
        var consulta = DeLaEmpresa();

        if (request.Activo is bool activo)
            consulta = consulta.Where(cliente => cliente.Activo == activo);

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLower();
            consulta = consulta.Where(cliente =>
                (cliente.Nombre + " " + cliente.Apellido).ToLower().Contains(texto)
                || cliente.Email.ToLower().Contains(texto)
                || cliente.Telefono.ToLower().Contains(texto));
        }

        return consulta
            .OrderBy(cliente => cliente.Nombre)
            .ThenBy(cliente => cliente.Apellido)
            .ThenBy(cliente => cliente.Id)
            .PaginarAsync(request);
    }

    public async Task<ResumenClientesResponse> GetResumenAsync()
        => new()
        {
            Total = await DeLaEmpresa().CountAsync(),
            Activos = await DeLaEmpresa().CountAsync(cliente => cliente.Activo)
        };

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
