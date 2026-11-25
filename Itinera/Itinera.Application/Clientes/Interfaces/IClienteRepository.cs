using Itinera.Application.Common.Interfaces;
using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteRepository : IPaginadoRepository<Cliente, ListadoActivableRequest>
{
    Task AddAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByIdConInactivosAsync(int id);
    Task<List<Cliente>> GetAllAsync();
    Task<ResumenClientesResponse> GetResumenAsync();
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);
}
