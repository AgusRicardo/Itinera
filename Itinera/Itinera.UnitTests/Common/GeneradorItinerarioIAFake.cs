using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Models;

namespace Itinera.UnitTests.Common;

public sealed class GeneradorItinerarioIAFake : IGeneradorItinerarioIA
{
    public Task<IReadOnlyList<ItinerarioGenerado>> GenerarAsync(
        SolicitudItinerario solicitud,
        CancellationToken cancellationToken = default)
    {
        var destinos = solicitud.Destinos;

        if (destinos.Count == 0)
            return Task.FromResult<IReadOnlyList<ItinerarioGenerado>>(new[]
            {
                new ItinerarioGenerado(
                    "Itinerario generado automáticamente",
                    "Alternativa generada con el catálogo disponible.",
                    0,
                    Array.Empty<DestinoPlanificado>())
            });

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
            var diasDelDestino = Math.Max(1, partida.DayNumber - llegada.DayNumber + 1);

            if (partida > solicitud.FechaFin)
                partida = solicitud.FechaFin;
            if (partida < llegada)
                partida = llegada;

            var actividades = destino.Actividades
                .Select((actividad, posicion) => new ActividadPlanificada(
                    actividad.ActividadId,
                    actividad.Nombre,
                    actividad.Descripcion,
                    actividad.CostoBase,
                    actividad.DuracionEstimada,
                    CalcularInicio(destino.Actividades, posicion, llegada, diasDelDestino, solicitud),
                    actividad.CostoBase * solicitud.CantidadViajeros,
                    string.Empty,
                    posicion + 1))
                .ToList();

            for (var dia = llegada; dia <= partida; dia = dia.AddDays(1))
            {
                if (actividades.All(actividad => DateOnly.FromDateTime(actividad.FechaHoraInicio) != dia))
                    actividades.Add(CrearRecomendada(
                        $"Paseo {dia:dd-MM} por {destino.Ciudad}",
                        destino,
                        dia.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc)));
            }

            planificados.Add(new DestinoPlanificado(indice + 1, llegada, partida, destino.DestinoId, actividades));
            cursor = partida.AddDays(1);
        }

        return Task.FromResult<IReadOnlyList<ItinerarioGenerado>>(new[]
        {
            new ItinerarioGenerado(
                "Itinerario generado automáticamente",
                "Alternativa generada con el catálogo disponible.",
                planificados.SelectMany(destino => destino.Actividades).Sum(actividad => actividad.CostoFinal),
                planificados)
        });
    }

    private static DateTime CalcularInicio(
        IReadOnlyList<ActividadSolicitada> actividades,
        int posicion,
        DateOnly llegada,
        int dias,
        SolicitudItinerario solicitud)
    {
        var minutosOcupados = actividades
            .Take(posicion)
            .Where((_, indice) => indice % dias == posicion % dias)
            .Sum(actividad => actividad.DuracionEstimada
                + solicitud.Perfil.MinutosTrasladoEntreActividades);

        return llegada.AddDays(posicion % dias)
            .ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc)
            .AddMinutes(minutosOcupados);
    }

    public Task<IReadOnlyList<ModificacionDiaGenerada>> GenerarModificacionDiaAsync(
        SolicitudModificacionDia solicitud,
        CancellationToken cancellationToken = default)
    {
        var destino = solicitud.Destinos[0];
        var actividades = solicitud.Dias
            .Where(dia => dia.Fecha == solicitud.FechaObjetivo)
            .SelectMany(dia => dia.Actividades)
            .ToList();
        actividades.Add(CrearRecomendada(
            $"Recorrido sugerido {solicitud.FechaObjetivo:dd-MM}",
            destino,
            solicitud.FechaObjetivo.ToDateTime(new TimeOnly(18, 0), DateTimeKind.Utc)));

        return Task.FromResult<IReadOnlyList<ModificacionDiaGenerada>>(new[]
        {
            new ModificacionDiaGenerada(
                solicitud.FechaObjetivo,
                "Se suma un recorrido por la tarde.",
                0,
                actividades)
        });
    }

    private static ActividadPlanificada CrearRecomendada(
        string nombre,
        DestinoSolicitado destino,
        DateTime inicio)
        => new(
            null,
            nombre,
            $"Actividad de ejemplo en {destino.Ciudad}.",
            20,
            60,
            inicio,
            0,
            string.Empty,
            99,
            destino.DestinoId);
}
