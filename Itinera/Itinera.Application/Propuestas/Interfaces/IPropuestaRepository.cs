using Itinera.Application.Common.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IPropuestaRepository : IPaginadoRepository<Propuesta, ListadoComercialRequest>
{
    Task AddAsync(Propuesta propuesta);
    Task<Propuesta?> GetByIdAsync(int id);
    Task<List<Propuesta>> GetAllAsync();
    Task<ResumenPropuestasResponse> GetResumenAsync();
    Task<List<Propuesta>> GetByClienteAsync(int clienteId);
    Task UpdateAsync(Propuesta propuesta);
}
