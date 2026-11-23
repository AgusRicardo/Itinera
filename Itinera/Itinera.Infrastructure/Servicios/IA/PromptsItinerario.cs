using System.Text.Encodings.Web;
using System.Text.Json;
using Itinera.Application.Propuestas.Models;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Common;

namespace Itinera.Infrastructure.Servicios.IA;

public static class PromptsItinerario
{
    private static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private const string FormatoActividad = """
        Cada actividad es un array compacto: ["HH:mm", id, "nombre", minutos, costoPorPersonaUSD]. Sin texto extra.
        - id: el de la actividad obligatoria, o null si es recomendada.
        - nombre: específico y de hasta 8 palabras. costo: 0 si es gratuita.
        """;

    private const string ReglasActividades = """
        - Las "obligatorias" las pidió el cliente: incluí todas, una sola vez y con su id. Podés elegirles día y horario, pero no omitirlas ni reemplazarlas.
        - El resto del tiempo completalo con actividades recomendadas, reales y propias del destino o excursiones cercanas.
        - Las preferencias son el interés principal, no el único: si no alcanzan para cubrir el ritmo, completá con gastronomía, barrios, museos, parques, miradores, mercados o excursiones.
        - No repitas actividades ni superpongas horarios.
        """;

    public static string ConstruirGeneracion(SolicitudItinerario solicitud, int cantidadAlternativas)
        => $$"""
        Sos un planificador profesional de viajes. Generá exactamente {{cantidadAlternativas}} alternativas distintas de itinerario.
        Respondé únicamente JSON válido, sin espacios ni saltos de línea innecesarios, con esta forma:
        {"alternativas":[{"nombre":"","descripcion":"una oración","dias":{"yyyy-MM-dd":[["09:00",null,"nombre",120,0]]} }]}
        {{FormatoActividad}}

        Reglas obligatorias:
        - "dias" debe tener una clave por cada una de estas fechas: {{DescribirFechas(solicitud)}}.
        {{DescribirRitmo(solicitud)}}
        {{ReglasActividades}}
        - El costo total de cada alternativa (costo por persona por {{solicitud.CantidadViajeros}} viajeros) no puede superar el presupuesto. Si no alcanza, usá actividades gratuitas antes que dejar días libres.
        {{DescribirGasto(solicitud)}}

        Datos:
        {{DescribirDatos(solicitud)}}
        """;

    public static string ConstruirModificacion(SolicitudModificacionDia solicitud)
        => $$"""
        Sos un planificador profesional de viajes. Proponé hasta 2 formas de modificar únicamente el día {{solicitud.FechaObjetivo:yyyy-MM-dd}} según las instrucciones.
        Respondé únicamente JSON válido con esta forma:
        {"alternativas":[{"descripcion":"una oración","actividades":[["09:00",null,"nombre",120,0]]}]}
        {{FormatoActividad}}

        Reglas obligatorias:
        - Devolvé el día completo: las actividades que se mantienen y las nuevas.
        - Hasta {{solicitud.Perfil.MaxActividadesPorDia}} actividades y {{solicitud.Perfil.MinutosDisponibles}} minutos en total, sumando {{solicitud.Perfil.MinutosTrasladoEntreActividades}} de traslado por actividad.
        {{ReglasActividades}}
        - Las obligatorias deben permanecer aunque las instrucciones pidan quitarlas.
        - El costo por persona por {{solicitud.CantidadViajeros}} viajeros no puede superar {{solicitud.PresupuestoRestante:0}} USD.

        Datos:
        {{JsonSerializer.Serialize(new
        {
            solicitud.Instrucciones,
            solicitud.Preferencias,
            solicitud.Etiquetas,
            destino = DescribirDestino(solicitud.Destinos),
            obligatorias = DescribirObligatorias(solicitud.Destinos),
            diaActual = solicitud.Dias
                .Where(dia => dia.Fecha == solicitud.FechaObjetivo)
                .SelectMany(dia => dia.Actividades)
                .Select(actividad => new
                {
                    hora = actividad.FechaHoraInicio.ToString("HH:mm"),
                    actividad.Nombre,
                    minutos = actividad.DuracionEstimada,
                    costo = actividad.CostoBase
                }),
            actividadesDeOtrosDias = solicitud.Dias
                .Where(dia => dia.Fecha != solicitud.FechaObjetivo)
                .SelectMany(dia => dia.Actividades)
                .Select(actividad => actividad.Nombre)
        }, OpcionesJson)}}
        """;

    private static string DescribirGasto(SolicitudItinerario solicitud)
    {
        var objetivo = Math.Min(solicitud.PresupuestoObjetivo ?? solicitud.Presupuesto, solicitud.Presupuesto);
        var actividades = Dias(solicitud) * Math.Max(1, solicitud.Perfil.MinActividadesPorDia);
        var promedio = objetivo * 0.8m / actividades / solicitud.CantidadViajeros;

        return $"""
            - Aprovechá el presupuesto: cada alternativa debería costar en total entre {objetivo * 0.6m:0} y {objetivo * 0.95m:0} USD, unos {promedio:0} USD por persona en promedio por actividad.
            - Para lograrlo incluí experiencias pagas de calidad: excursiones de día completo, guías privados, gastronomía destacada, espectáculos, talleres, spa o paseos panorámicos. Usá precios realistas, sin inflarlos.
            """;
    }

    private static string DescribirRitmo(SolicitudItinerario solicitud)
    {
        var perfil = solicitud.Perfil;
        var diasLibres = perfil.MaxDiasLibres(Dias(solicitud));
        var libres = diasLibres == 0
            ? "Ningún día puede quedar vacío."
            : $"Como máximo {diasLibres} días pueden quedar vacíos ([]), repartidos.";

        return $"""
            - Intensidad {solicitud.IntensidadViaje.ToString().ToUpperInvariant()}: planificá entre {perfil.MinActividadesPorDia} y {perfil.MaxActividadesPorDia} actividades por día. {libres}
            - Cada día puede ocupar hasta {perfil.MinutosDisponibles} minutos, sumando {perfil.MinutosTrasladoEntreActividades} de traslado por actividad y {perfil.MinutosPausaDescanso} de pausa. El primer día resta 90 minutos por la llegada.
            """;
    }

    private static string DescribirFechas(SolicitudItinerario solicitud)
        => string.Join(", ", Enumerable.Range(0, Dias(solicitud))
            .Select(dia => solicitud.FechaInicio.AddDays(dia).ToString("yyyy-MM-dd")));

    private static string DescribirDatos(SolicitudItinerario solicitud)
        => JsonSerializer.Serialize(new
        {
            destino = DescribirDestino(solicitud.Destinos),
            presupuestoUSD = solicitud.Presupuesto,
            viajeros = solicitud.CantidadViajeros,
            solicitud.Preferencias,
            solicitud.Etiquetas,
            obligatorias = DescribirObligatorias(solicitud.Destinos)
        }, OpcionesJson);

    private static string DescribirDestino(IReadOnlyList<DestinoSolicitado> destinos)
        => string.Join(", ", destinos.Select(destino => $"{destino.Ciudad}, {destino.Pais}"));

    private static object DescribirObligatorias(IReadOnlyList<DestinoSolicitado> destinos)
        => destinos
            .SelectMany(destino => destino.Actividades)
            .Select(actividad => new
            {
                id = actividad.ActividadId,
                actividad.Nombre,
                minutos = actividad.DuracionEstimada,
                costo = actividad.CostoBase
            });

    private static int Dias(SolicitudItinerario solicitud)
        => solicitud.FechaFin.DayNumber - solicitud.FechaInicio.DayNumber + 1;
}
