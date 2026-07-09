using Itinera.Application.Propuestas.Dtos;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IPropuestaService
{
    Task<CrearPropuestaResponse> CrearAsync(CrearPropuestaRequest request);
}
