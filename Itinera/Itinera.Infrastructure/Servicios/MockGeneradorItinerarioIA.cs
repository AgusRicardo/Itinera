using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Models;

namespace Itinera.Infrastructure.Servicios;

public class MockGeneradorItinerarioIA : IGeneradorItinerarioIA
{
    private const int MaxActividadesPorDestino = 2;

    public Task<ItinerarioGenerado> GenerarAsync(
        SolicitudItinerario solicitud,
        CancellationToken cancellationToken = default)
    {
        var destinos = solicitud.Destinos;

        if (destinos.Count == 0)
            return Task.FromResult(new ItinerarioGenerado(
                "Itinerario generado automáticamente",
                Array.Empty<DestinoPlanificado>()));

        var diasTotales = Math.Max(1, solicitud.FechaFin.DayNumber - solicitud.FechaInicio.DayNumber + 1);
        var diasPorDestino = Math.Max(1, diasTotales / destinos.Count);

        var planificados = new List<DestinoPlanificado>(destinos.Count);
        var cursor = solicitud.FechaInicio;

        for (var indice = 0; indice < destinos.Count; indice++)
        {
            var destino = destinos[indice];
            var llegada = cursor;
            var esUltimo = indice == destinos.Count - 1;
            var partida = esUltimo ? solicitud.FechaFin : llegada.AddDays(diasPorDestino - 1);

            if (partida > solicitud.FechaFin)
                partida = solicitud.FechaFin;
            if (partida < llegada)
                partida = llegada;

            var actividades = destino.Actividades
                .Take(MaxActividadesPorDestino)
                .Select((actividad, posicion) => new ActividadPlanificada(
                    actividad,
                    llegada.ToDateTime(new TimeOnly(9 + posicion * 5, 0), DateTimeKind.Utc),
                    actividad.CostoBase,
                    string.Empty,
                    posicion + 1))
                .ToList();

            planificados.Add(new DestinoPlanificado(indice + 1, llegada, partida, destino, actividades));
            cursor = partida.AddDays(1);
        }

        return Task.FromResult(new ItinerarioGenerado(
            "Itinerario generado automáticamente",
            planificados));
    }
}
