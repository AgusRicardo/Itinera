using Itinera.Application.Clientes.Interfaces;
using Itinera.Domain.Common;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class EmpresaClienteRepository(AppDbContext context) : IEmpresaClienteRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(EmpresaCliente empresaCliente)
    {
        _context.EmpresaClientes.Add(empresaCliente);
        return _context.SaveChangesAsync();
    }
}
