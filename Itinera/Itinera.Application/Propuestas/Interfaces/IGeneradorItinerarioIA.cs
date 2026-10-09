using Itinera.Application.Propuestas.Models;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IGeneradorItinerarioIA
{
    Task<ItinerarioGenerado> GenerarAsync(
        SolicitudItinerario solicitud,
        CancellationToken cancellationToken = default);
}
