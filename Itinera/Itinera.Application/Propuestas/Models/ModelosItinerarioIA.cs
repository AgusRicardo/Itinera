using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Models;

public sealed record SolicitudItinerario(
    DateOnly FechaInicio,
    DateOnly FechaFin,
    decimal Presupuesto,
    string Preferencias,
    IReadOnlyList<Destino> Destinos);

public sealed record ItinerarioGenerado(
    string Nombre,
    IReadOnlyList<DestinoPlanificado> Destinos);

public sealed record DestinoPlanificado(
    int Orden,
    DateOnly FechaLlegada,
    DateOnly FechaPartida,
    Destino Destino,
    IReadOnlyList<ActividadPlanificada> Actividades);

public sealed record ActividadPlanificada(
    Actividad Actividad,
    DateTime FechaHoraInicio,
    decimal CostoFinal,
    string Observaciones,
    int Orden);
