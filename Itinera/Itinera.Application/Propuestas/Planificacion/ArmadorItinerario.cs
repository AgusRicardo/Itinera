using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Planificacion;

public sealed class ArmadorItinerario(IActividadRepository actividadRepository)
{
    public async Task<List<ActividadItinerario>> ConstruirAsync(
        Propuesta propuesta,
        int destinoId,
        IEnumerable<ActividadItinerarioRequest> solicitadas)
    {
        var destino = propuesta.ObtenerDestino(destinoId)
            ?? throw new DestinoNoEncontradoException(destinoId);
        var actividades = new List<ActividadItinerario>();

        foreach (var solicitada in solicitadas.OrderBy(item => item.FechaHoraInicio))
        {
            var fechaHora = NormalizarUtc(solicitada.FechaHoraInicio);
            if (!destino.Incluye(DateOnly.FromDateTime(fechaHora)))
                throw new ArgumentException("Hay una actividad fuera de las fechas del destino.");

            var actividadId = solicitada.ActividadId
                ?? throw new ArgumentException("El itinerario contiene una actividad sin guardar.");
            var actividad = await actividadRepository.GetByIdAsync(actividadId)
                ?? throw new ActividadNoEncontradaException(actividadId);

            actividades.Add(new ActividadItinerario(
                actividad,
                fechaHora,
                propuesta.CantidadViajeros,
                solicitada.Observaciones,
                actividades.Count + 1));
        }

        if (actividades.DistinctBy(item => item.ActividadId).Count() != actividades.Count)
            throw new ArgumentException("El itinerario tiene actividades repetidas.");

        return actividades;
    }

    private static DateTime NormalizarUtc(DateTime fecha)
        => fecha.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
            : fecha.ToUniversalTime();
}
