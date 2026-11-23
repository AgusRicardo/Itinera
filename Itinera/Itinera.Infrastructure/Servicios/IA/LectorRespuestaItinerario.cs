using System.Globalization;
using System.Text.Json;
using Itinera.Application.Propuestas.Models;

namespace Itinera.Infrastructure.Servicios.IA;

public static class LectorRespuestaItinerario
{
    public static IReadOnlyList<ItinerarioGenerado>? LeerGeneracion(string json, SolicitudItinerario solicitud)
        => LeerAlternativas(json, alternativa => new ItinerarioGenerado(
            LeerTexto(alternativa, "nombre"),
            LeerTexto(alternativa, "descripcion"),
            0,
            [
                new DestinoPlanificado(
                    1,
                    solicitud.FechaInicio,
                    solicitud.FechaFin,
                    solicitud.Destinos[0].DestinoId,
                    LeerDias(alternativa))
            ]));

    public static IReadOnlyList<ModificacionDiaGenerada>? LeerModificacion(
        string json,
        SolicitudModificacionDia solicitud)
        => LeerAlternativas(json, alternativa => new ModificacionDiaGenerada(
            solicitud.FechaObjetivo,
            LeerTexto(alternativa, "descripcion"),
            0,
            alternativa.TryGetProperty("actividades", out var actividades)
                ? LeerActividades(actividades, solicitud.FechaObjetivo)
                : []));

    private static List<T>? LeerAlternativas<T>(string json, Func<JsonElement, T> leer)
    {
        try
        {
            using var documento = JsonDocument.Parse(json);

            return documento.RootElement.ValueKind == JsonValueKind.Object
                && documento.RootElement.TryGetProperty("alternativas", out var alternativas)
                && alternativas.ValueKind == JsonValueKind.Array
                    ? alternativas.EnumerateArray()
                        .Where(alternativa => alternativa.ValueKind == JsonValueKind.Object)
                        .Select(leer)
                        .ToList()
                    : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static List<ActividadPlanificada> LeerDias(JsonElement alternativa)
        => alternativa.TryGetProperty("dias", out var dias) && dias.ValueKind == JsonValueKind.Object
            ? dias.EnumerateObject()
                .SelectMany(dia => DateOnly.TryParseExact(
                    dia.Name, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha)
                        ? LeerActividades(dia.Value, fecha)
                        : [])
                .ToList()
            : [];

    private static List<ActividadPlanificada> LeerActividades(JsonElement actividades, DateOnly fecha)
        => actividades.ValueKind == JsonValueKind.Array
            ? actividades.EnumerateArray()
                .Select(actividad => LeerActividad(actividad, fecha))
                .OfType<ActividadPlanificada>()
                .ToList()
            : [];

    private static ActividadPlanificada? LeerActividad(JsonElement actividad, DateOnly fecha)
    {
        if (actividad.ValueKind != JsonValueKind.Array || actividad.GetArrayLength() < 5)
            return null;

        var nombre = actividad[2].ValueKind == JsonValueKind.String ? actividad[2].GetString() : null;

        if (nombre is null
            || !TimeOnly.TryParse(actividad[0].ToString(), CultureInfo.InvariantCulture, out var hora)
            || LeerNumero(actividad[3]) is not decimal minutos
            || LeerNumero(actividad[4]) is not decimal costo
            || minutos is < 1 or > 1440
            || costo is < 0 or > 1_000_000)
            return null;

        return new ActividadPlanificada(
            LeerNumero(actividad[1]) is decimal id and > 0 and < int.MaxValue ? (int)id : null,
            nombre,
            string.Empty,
            costo,
            (int)minutos,
            fecha.ToDateTime(hora, DateTimeKind.Utc),
            0,
            string.Empty,
            0);
    }

    private static decimal? LeerNumero(JsonElement valor)
        => decimal.TryParse(valor.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var numero)
            ? numero
            : null;

    private static string LeerTexto(JsonElement objeto, string propiedad)
        => objeto.TryGetProperty(propiedad, out var valor) && valor.ValueKind == JsonValueKind.String
            ? valor.GetString() ?? string.Empty
            : string.Empty;
}
