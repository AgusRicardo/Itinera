using Itinera.Application.Propuestas.Models;

namespace Itinera.Application.Propuestas.Planificacion;

public sealed class ValidadorItinerarioGenerado(
    PlanificacionItinerarioOptions opciones)
{
    public const int LargoMaximoNombre = 150;

    public bool EsValido(ItinerarioGenerado itinerario, SolicitudItinerario solicitud)
    {
        if (itinerario.Destinos.Count == 0
            || itinerario.CostoEstimado > solicitud.Presupuesto
            || !IncluyeActividadesObligatorias(itinerario, solicitud))
            return false;

        var destinosSolicitados = solicitud.Destinos
            .Select(destino => destino.DestinoId)
            .ToHashSet();
        var perfil = solicitud.Perfil;

        foreach (var destino in itinerario.Destinos)
        {
            if (!destinosSolicitados.Contains(destino.DestinoId))
                return false;

            if (destino.FechaLlegada < solicitud.FechaInicio
                || destino.FechaPartida > solicitud.FechaFin
                || destino.FechaPartida < destino.FechaLlegada
                || destino.Actividades.Count > perfil.MaxActividadesPorDia * Dias(destino))
                return false;

            if (destino.Actividades.Any(actividad =>
                    string.IsNullOrWhiteSpace(actividad.Nombre)
                    || actividad.Nombre.Length > LargoMaximoNombre
                    || actividad.DuracionEstimada <= 0
                    || actividad.CostoFinal < 0
                    || !EstaDentroDeLaEstadia(actividad, destino)))
                return false;

            var actividadesPorDia = destino.Actividades
                .GroupBy(actividad => DateOnly.FromDateTime(actividad.FechaHoraInicio))
                .ToList();

            foreach (var dia in actividadesPorDia)
            {
                var actividades = dia.OrderBy(actividad => actividad.FechaHoraInicio).ToList();
                if (actividades.Count > perfil.MaxActividadesPorDia)
                    return false;

                var ocupacion = perfil.MinutosOcupados(
                    actividades.Select(actividad => actividad.DuracionEstimada).ToList(),
                    dia.Key == destino.FechaLlegada ? opciones.MinutosTrasladoPorDestino : 0);

                if (ocupacion > perfil.MinutosDisponibles)
                    return false;

                for (var indice = 1; indice < actividades.Count; indice++)
                {
                    var finAnterior = actividades[indice - 1].FechaHoraInicio
                        .AddMinutes(actividades[indice - 1].DuracionEstimada);
                    if (actividades[indice].FechaHoraInicio < finAnterior)
                        return false;
                }
            }
        }

        return true;
    }

    private static bool IncluyeActividadesObligatorias(
        ItinerarioGenerado itinerario,
        SolicitudItinerario solicitud)
        => solicitud.Destinos.All(solicitado => solicitado.Actividades.All(obligatoria =>
            itinerario.Destinos
                .Where(destino => destino.DestinoId == solicitado.DestinoId)
                .SelectMany(destino => destino.Actividades)
                .Count(actividad => actividad.ActividadId == obligatoria.ActividadId) == 1));

    private static bool EstaDentroDeLaEstadia(ActividadPlanificada actividad, DestinoPlanificado destino)
    {
        var fecha = DateOnly.FromDateTime(actividad.FechaHoraInicio);

        return fecha >= destino.FechaLlegada && fecha <= destino.FechaPartida;
    }

    private static int Dias(DestinoPlanificado destino)
        => destino.FechaPartida.DayNumber - destino.FechaLlegada.DayNumber + 1;
}
