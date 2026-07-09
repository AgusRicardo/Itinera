using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteRepository
{
    Task AddAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(int id);
}
