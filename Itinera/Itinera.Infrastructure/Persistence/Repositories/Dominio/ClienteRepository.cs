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
        => _context.Clientes
            .FirstOrDefaultAsync(cliente => cliente.Id == id
                && (EsAdministradorPlataforma
                    || (EmpresaId != null && cliente.Empresas.Any(ec => ec.EmpresaId == EmpresaId))));

    public Task<List<Cliente>> GetAllAsync()
        => _context.Clientes
            .Where(cliente => EsAdministradorPlataforma
                || (EmpresaId != null && cliente.Empresas.Any(ec => ec.EmpresaId == EmpresaId)))
            .ToListAsync();

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
