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
        var anterior = propuesta.ObtenerItinerario(destinoId);

        foreach (var solicitada in solicitadas.OrderBy(item => item.FechaHoraInicio))
        {
            var fechaHora = NormalizarUtc(solicitada.FechaHoraInicio);
            if (!destino.Incluye(DateOnly.FromDateTime(fechaHora)))
                throw new ArgumentException("Hay una actividad fuera de las fechas del destino.");

            var actividadId = solicitada.ActividadId
                ?? throw new ArgumentException("El itinerario contiene una actividad sin guardar.");
            var actividad = await actividadRepository.GetByIdConInactivasAsync(actividadId);
            if (actividad is null || (!actividad.Activo && !propuesta.UsaActividad(actividadId)))
                throw new ActividadNoEncontradaException(actividadId);

            actividades.Add(new ActividadItinerario(
                actividad,
                fechaHora,
                propuesta.CantidadViajeros,
                solicitada.Observaciones,
                actividades.Count + 1,
                solicitada.RecomendadaPorIA
                    || anterior?.Actividades.Any(item => item.ActividadId == actividadId && item.RecomendadaPorIA) == true));
        }

        if (actividades.DistinctBy(item => item.ActividadId).Count() != actividades.Count)
            throw new ArgumentException("El itinerario tiene actividades repetidas.");

        ValidarSuperposiciones(actividades);

        return actividades;
    }

    private static void ValidarSuperposiciones(List<ActividadItinerario> actividades)
    {
        ActividadItinerario? anterior = null;

        foreach (var actividad in actividades.OrderBy(item => item.FechaHoraInicio))
        {
            if (anterior is not null && actividad.FechaHoraInicio < anterior.FechaHoraFin)
                throw new ArgumentException(
                    $"{anterior.Actividad.Nombre} y {actividad.Actividad.Nombre} se superponen. Cambiá el horario de una de ellas.");

            anterior = actividad;
        }
    }

    private static DateTime NormalizarUtc(DateTime fecha)
        => fecha.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
            : fecha.ToUniversalTime();
}
