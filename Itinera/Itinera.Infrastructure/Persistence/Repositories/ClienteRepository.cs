using Itinera.Application.Clientes.Interfaces;
using Itinera.Domain.Usuarios;

namespace Itinera.Infrastructure.Persistence.Repositories;

public class ClienteRepository(AppDbContext context) : IClienteRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Cliente cliente)
    {
        throw new NotImplementedException();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }
}
