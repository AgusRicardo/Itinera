using Itinera.Application.Common.Interfaces;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Empresa;

namespace Itinera.Application.Empresas.Interfaces;

public interface IEmpresaRepository : IPaginadoRepository<Empresa, ListadoActivableRequest>
{
    Task AddAsync(Empresa empresa);
    Task<Empresa?> GetByIdAsync(int id);
    Task<Empresa?> GetByIdConInactivasAsync(int id);
    Task<List<Empresa>> GetAllConInactivasAsync();
    Task UpdateAsync(Empresa empresa);
    Task DeleteAsync(Empresa empresa);
}
