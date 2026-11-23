using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Usuarios;

namespace Itinera.Application.Clientes.Interfaces;

public interface IClienteRepository
{
    Task AddAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(int id);
    Task<List<Cliente>> GetAllAsync();
    Task<Pagina<Cliente>> GetPaginaAsync(ListarClientesRequest request);
    Task<ResumenClientesResponse> GetResumenAsync();
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);
}
