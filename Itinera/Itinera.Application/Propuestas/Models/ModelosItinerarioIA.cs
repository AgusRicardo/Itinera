using Itinera.Domain.Common;
using Itinera.Application.Propuestas.Planificacion;

namespace Itinera.Application.Propuestas.Models;

public sealed record SolicitudItinerario(
    DateOnly FechaInicio,
    DateOnly FechaFin,
    decimal Presupuesto,
    int CantidadViajeros,
    string Preferencias,
    IReadOnlyList<string> Etiquetas,
    IntensidadViaje IntensidadViaje,
    PerfilIntensidad Perfil,
    IReadOnlyList<DestinoSolicitado> Destinos,
    decimal? PresupuestoObjetivo = null);

public sealed record DestinoSolicitado(
    int DestinoId,
    string Ciudad,
    string Pais,
    IReadOnlyList<ActividadSolicitada> Actividades);

public sealed record ActividadSolicitada(
    int ActividadId,
    string Nombre,
    string Descripcion,
    decimal CostoBase,
    int DuracionEstimada);

public sealed record ItinerarioGenerado(
    string Nombre,
    string Descripcion,
    decimal CostoEstimado,
    IReadOnlyList<DestinoPlanificado> Destinos);

public sealed record DestinoPlanificado(
    int Orden,
    DateOnly FechaLlegada,
    DateOnly FechaPartida,
    int DestinoId,
    IReadOnlyList<ActividadPlanificada> Actividades);

public sealed record ActividadPlanificada(
    int? ActividadId,
    string Nombre,
    string Descripcion,
    decimal CostoBase,
    int DuracionEstimada,
    DateTime FechaHoraInicio,
    decimal CostoFinal,
    string Observaciones,
    int Orden,
    int? DestinoId = null);

public sealed record SolicitudModificacionDia(
    DateOnly FechaObjetivo,
    string Instrucciones,
    decimal PresupuestoRestante,
    int CantidadViajeros,
    string Preferencias,
    IReadOnlyList<string> Etiquetas,
    IntensidadViaje IntensidadViaje,
    PerfilIntensidad Perfil,
    IReadOnlyList<DiaItinerarioContexto> Dias,
    IReadOnlyList<DestinoSolicitado> Destinos);

public sealed record DiaItinerarioContexto(
    DateOnly Fecha,
    int? DestinoId,
    string Ciudad,
    IReadOnlyList<ActividadPlanificada> Actividades);

public sealed record ModificacionDiaGenerada(
    DateOnly Fecha,
    string Descripcion,
    decimal CostoEstimado,
    IReadOnlyList<ActividadPlanificada> Actividades);
