using System.Globalization;
using Itinera.Application.Propuestas.Models;

namespace Itinera.Application.Propuestas.Planificacion;

/// <summary>
/// Acomoda lo que propone la IA a los límites del viaje quitando solo actividades recomendadas;
/// las pedidas por el cliente nunca se quitan.
/// </summary>
public sealed class AjustadorItinerarioGenerado(PlanificacionItinerarioOptions opciones)
{
    public ItinerarioGenerado Ajustar(ItinerarioGenerado itinerario, SolicitudItinerario solicitud)
    {
        var pedidas = solicitud.Destinos
            .SelectMany(destino => destino.Actividades)
            .Select(actividad => actividad.ActividadId)
            .ToHashSet();
        var destinos = itinerario.Destinos
            .Select(destino => destino with
            {
                Actividades = AjustarDias(destino, solicitud.Perfil, pedidas)
            })
            .ToList();

        AjustarPresupuesto(destinos, solicitud.Presupuesto, pedidas);

        return itinerario with
        {
            Destinos = destinos,
            CostoEstimado = destinos
                .SelectMany(destino => destino.Actividades)
                .Sum(actividad => actividad.CostoFinal)
        };
    }

    public int ContarDiasLibres(ItinerarioGenerado itinerario, SolicitudItinerario solicitud)
        => Dias(solicitud) - itinerario.Destinos
            .SelectMany(destino => destino.Actividades)
            .Select(actividad => DateOnly.FromDateTime(actividad.FechaHoraInicio))
            .Distinct()
            .Count();

    public bool CumpleRitmo(ItinerarioGenerado itinerario, SolicitudItinerario solicitud)
        => ContarDiasLibres(itinerario, solicitud) <= solicitud.Perfil.MaxDiasLibres(Dias(solicitud));

    public bool EsAceptable(ItinerarioGenerado itinerario, SolicitudItinerario solicitud)
        => ContarDiasLibres(itinerario, solicitud)
            <= solicitud.Perfil.MaxDiasLibres(Dias(solicitud)) + (Dias(solicitud) + 9) / 10;

    public List<ItinerarioGenerado> Elegir(
        IReadOnlyCollection<ItinerarioGenerado> validas,
        SolicitudItinerario solicitud)
    {
        var conRitmo = validas.Where(item => CumpleRitmo(item, solicitud)).ToList();
        var elegidas = conRitmo.Count > 0
            ? conRitmo
            : validas.Where(item => EsAceptable(item, solicitud)).ToList();

        return elegidas
            .OrderBy(item => ContarDiasLibres(item, solicitud))
            .ThenBy(item => Math.Abs((solicitud.PresupuestoObjetivo ?? 0) - item.CostoEstimado))
            .ToList();
    }

    private List<ActividadPlanificada> AjustarDias(
        DestinoPlanificado destino,
        PerfilIntensidad perfil,
        HashSet<int> pedidas)
        => destino.Actividades
            .Where(actividad => EsPedida(actividad, pedidas) || EsRecomendacionValida(actividad, destino))
            .OrderByDescending(actividad => EsPedida(actividad, pedidas))
            .ThenBy(actividad => actividad.FechaHoraInicio)
            .DistinctBy(actividad => actividad.ActividadId?.ToString(CultureInfo.InvariantCulture)
                ?? actividad.Nombre.ToLowerInvariant())
            .GroupBy(actividad => DateOnly.FromDateTime(actividad.FechaHoraInicio))
            .SelectMany(dia => AjustarDia(
                dia.OrderBy(actividad => actividad.FechaHoraInicio).ToList(),
                dia.Key == destino.FechaLlegada,
                perfil,
                pedidas))
            .OrderBy(actividad => actividad.FechaHoraInicio)
            .ToList();

    private List<ActividadPlanificada> AjustarDia(
        List<ActividadPlanificada> actividades,
        bool esDiaDeLlegada,
        PerfilIntensidad perfil,
        HashSet<int> pedidas)
    {
        while (actividades.Count > perfil.MaxActividadesPorDia
            || MinutosOcupados(actividades, esDiaDeLlegada, perfil) > perfil.MinutosDisponibles)
        {
            var indice = actividades.FindLastIndex(actividad => !EsPedida(actividad, pedidas));
            if (indice < 0)
                break;

            actividades.RemoveAt(indice);
        }

        var apertura = LeerHora(opciones.HoraApertura);
        var cierre = LeerHora(opciones.HoraCierre);
        var acomodadas = new List<ActividadPlanificada>();
        DateTime? finAnterior = null;

        foreach (var actividad in actividades)
        {
            var inicio = actividad.FechaHoraInicio;
            if (inicio.TimeOfDay < apertura)
                inicio = inicio.Date.Add(apertura);

            if (finAnterior is DateTime fin
                && inicio < fin.AddMinutes(perfil.MinutosTrasladoEntreActividades))
                inicio = fin.AddMinutes(perfil.MinutosTrasladoEntreActividades);

            var terminaTarde = inicio.AddMinutes(actividad.DuracionEstimada)
                > actividad.FechaHoraInicio.Date.Add(cierre);
            if (terminaTarde && !EsPedida(actividad, pedidas))
                continue;

            acomodadas.Add(actividad with
            {
                FechaHoraInicio = DateTime.SpecifyKind(inicio, actividad.FechaHoraInicio.Kind)
            });
            finAnterior = inicio.AddMinutes(actividad.DuracionEstimada);
        }

        return acomodadas;
    }

    private static void AjustarPresupuesto(
        List<DestinoPlanificado> destinos,
        decimal presupuesto,
        HashSet<int> pedidas)
    {
        var actividades = destinos.SelectMany(destino => destino.Actividades).ToList();

        while (actividades.Sum(actividad => actividad.CostoFinal) > presupuesto)
        {
            var quitar = actividades
                .Where(actividad => !EsPedida(actividad, pedidas) && actividad.CostoFinal > 0)
                .OrderByDescending(actividad => actividades.Count(otra =>
                    otra.FechaHoraInicio.Date == actividad.FechaHoraInicio.Date) > 1)
                .ThenByDescending(actividad => actividad.CostoFinal)
                .FirstOrDefault();

            if (quitar is null)
                break;

            actividades.Remove(quitar);
        }

        for (var indice = 0; indice < destinos.Count; indice++)
            destinos[indice] = destinos[indice] with
            {
                Actividades = destinos[indice].Actividades.Where(actividades.Contains).ToList()
            };
    }

    private int MinutosOcupados(
        List<ActividadPlanificada> actividades,
        bool esDiaDeLlegada,
        PerfilIntensidad perfil)
        => perfil.MinutosOcupados(
            actividades.Select(actividad => actividad.DuracionEstimada).ToList(),
            esDiaDeLlegada ? opciones.MinutosTrasladoPorDestino : 0);

    private static bool EsPedida(ActividadPlanificada actividad, HashSet<int> pedidas)
        => actividad.ActividadId is int id && pedidas.Contains(id);

    private static bool EsRecomendacionValida(ActividadPlanificada actividad, DestinoPlanificado destino)
    {
        var fecha = DateOnly.FromDateTime(actividad.FechaHoraInicio);

        return !string.IsNullOrWhiteSpace(actividad.Nombre)
            && actividad.Nombre.Length <= ValidadorItinerarioGenerado.LargoMaximoNombre
            && actividad.DuracionEstimada > 0
            && actividad.CostoFinal >= 0
            && fecha >= destino.FechaLlegada
            && fecha <= destino.FechaPartida;
    }

    private static TimeSpan LeerHora(string hora)
        => TimeOnly.Parse(hora, CultureInfo.InvariantCulture).ToTimeSpan();

    private static int Dias(SolicitudItinerario solicitud)
        => solicitud.FechaFin.DayNumber - solicitud.FechaInicio.DayNumber + 1;
}
