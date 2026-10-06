using Itinera.Domain.Empresa;

namespace Itinera.Application.Cargos.Interfaces;

public interface ICargoRepository
{
    Task AddAsync(Cargo cargo);
    Task<Cargo?> GetByIdAsync(int id);
    Task<List<Cargo>> GetAllAsync();
    Task UpdateAsync(Cargo cargo);
    Task DeleteAsync(Cargo cargo);
}
