using Itinera.Application.Clientes.Interfaces;
using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence.Repositories.Dominio;

public class ClienteRepository(AppDbContext context) : IClienteRepository
{
    private readonly AppDbContext _context = context;

    public Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        return _context.SaveChangesAsync();
    }

    public Task<Cliente?> GetByIdAsync(int id)
        => _context.Clientes.FindAsync(id).AsTask();

    public Task<List<Cliente>> GetAllAsync()
        => _context.Clientes.ToListAsync();

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
