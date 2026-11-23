using Itinera.Application.Propuestas.Models;

namespace Itinera.Application.Propuestas.Interfaces;

public interface IGeneradorItinerarioIA
{
    Task<IReadOnlyList<ItinerarioGenerado>> GenerarAsync(
        SolicitudItinerario solicitud,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ModificacionDiaGenerada>> GenerarModificacionDiaAsync(
        SolicitudModificacionDia solicitud,
        CancellationToken cancellationToken = default);
}
