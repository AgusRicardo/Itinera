using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IPropuestaRepository
{
    Task AddAsync(Propuesta propuesta);
    Task<Propuesta?> GetByIdAsync(int id);
    Task<List<Propuesta>> GetByClienteAsync(int clienteId);
    Task UpdateAsync(Propuesta propuesta);
}
