using Itinera.Domain.Empresa;

namespace Itinera.Application.Empresas.Interfaces;

public interface IEmpresaRepository
{
    Task AddAsync(Empresa empresa);
    Task<Empresa?> GetByIdAsync(int id);
    Task<List<Empresa>> GetAllAsync();
    Task UpdateAsync(Empresa empresa);
    Task DeleteAsync(Empresa empresa);
}
