using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Models;

namespace Itinera.Infrastructure.Servicios.IA;

public sealed class GroqGeneradorItinerarioIA(
    HttpClient httpClient,
    GeneradorIaOptions opciones) : IGeneradorItinerarioIA
{
    private const int MaximoIntentos = 3;
    private const int TokensPorActividad = 35;
    private const string MensajeSaturado =
        "El servicio de IA está saturado en este momento. Intentá nuevamente en unos minutos.";
    private const string MensajeCuota =
        "Se alcanzó el límite de uso de la IA. Esperá unos minutos e intentá nuevamente.";
    private const string MensajeRespuestaInvalida =
        "La IA devolvió una respuesta que no se pudo interpretar. Intentá nuevamente.";

    private static readonly TimeSpan EsperaMaxima = TimeSpan.FromSeconds(10);
    private static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web);

    public Task<IReadOnlyList<ItinerarioGenerado>> GenerarAsync(
        SolicitudItinerario solicitud,
        CancellationToken cancellationToken = default)
        => PedirAsync(
            PromptsItinerario.ConstruirGeneracion(solicitud, CalcularCantidadAlternativas(solicitud)),
            json => LectorRespuestaItinerario.LeerGeneracion(json, solicitud),
            cancellationToken);

    public Task<IReadOnlyList<ModificacionDiaGenerada>> GenerarModificacionDiaAsync(
        SolicitudModificacionDia solicitud,
        CancellationToken cancellationToken = default)
        => PedirAsync(
            PromptsItinerario.ConstruirModificacion(solicitud),
            json => LectorRespuestaItinerario.LeerModificacion(json, solicitud),
            cancellationToken);

    private int CalcularCantidadAlternativas(SolicitudItinerario solicitud)
    {
        var dias = solicitud.FechaFin.DayNumber - solicitud.FechaInicio.DayNumber + 1;
        var tokensPorAlternativa = dias * solicitud.Perfil.MinActividadesPorDia * TokensPorActividad;
        var tokensUtiles = opciones.MaxTokensRespuesta * 80 / 100;

        return Math.Clamp(
            tokensUtiles / Math.Max(1, tokensPorAlternativa),
            1,
            Math.Max(1, opciones.CantidadAlternativas));
    }

    private async Task<T> PedirAsync<T>(
        string prompt,
        Func<string, T?> leer,
        CancellationToken cancellationToken)
        where T : class
    {
        ServicioIANoDisponibleException? ultimoError = null;

        foreach (var modelo in new[] { opciones.Modelo }.Concat(opciones.ModelosAlternativos))
        {
            try
            {
                return await PedirConModeloAsync(modelo, prompt, leer, cancellationToken);
            }
            catch (ServicioIANoDisponibleException error)
            {
                ultimoError = error;
            }
        }

        throw ultimoError ?? new ServicioIANoDisponibleException(MensajeSaturado);
    }

    private async Task<T> PedirConModeloAsync<T>(
        string modelo,
        string prompt,
        Func<string, T?> leer,
        CancellationToken cancellationToken)
        where T : class
    {
        for (var intento = 1; ; intento++)
        {
            using var solicitud = CrearSolicitud(modelo, prompt);
            using var respuesta = await EnviarAsync(solicitud, cancellationToken);
            var contenido = await respuesta.Content.ReadAsStringAsync(cancellationToken);

            if (respuesta.IsSuccessStatusCode && Interpretar(contenido, leer) is T resultado)
                return resultado;

            var error = CrearError(modelo, respuesta, contenido);
            var espera = ObtenerEspera(respuesta, intento);

            if (error is not ServicioIANoDisponibleException
                || intento == MaximoIntentos
                || espera > EsperaMaxima)
                throw error;

            await Task.Delay(espera, cancellationToken);
        }
    }

    private HttpRequestMessage CrearSolicitud(string modelo, string prompt)
    {
        var cuerpo = new Dictionary<string, object>
        {
            ["model"] = modelo,
            ["messages"] = new[] { new { role = "user", content = prompt } },
            ["temperature"] = opciones.Temperatura,
            ["max_completion_tokens"] = opciones.MaxTokensRespuesta,
            ["response_format"] = new { type = "json_object" }
        };

        if (!string.IsNullOrWhiteSpace(opciones.EsfuerzoRazonamiento))
            cuerpo["reasoning_effort"] = opciones.EsfuerzoRazonamiento;

        var solicitud = new HttpRequestMessage(
            HttpMethod.Post,
            $"{opciones.UrlBase.TrimEnd('/')}/chat/completions")
        {
            Content = JsonContent.Create(cuerpo)
        };
        solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opciones.ApiKey);

        return solicitud;
    }

    private async Task<HttpResponseMessage> EnviarAsync(
        HttpRequestMessage solicitud,
        CancellationToken cancellationToken)
    {
        try
        {
            return await httpClient.SendAsync(solicitud, cancellationToken);
        }
        catch (Exception error) when (
            error is HttpRequestException
            || (error is TaskCanceledException && !cancellationToken.IsCancellationRequested))
        {
            throw new ServicioIANoDisponibleException(MensajeSaturado, error);
        }
    }

    private static T? Interpretar<T>(string contenido, Func<string, T?> leer)
        where T : class
    {
        try
        {
            var texto = JsonSerializer.Deserialize<RespuestaChat>(contenido, OpcionesJson)?
                .Choices?.FirstOrDefault()?.Message?.Content;

            return string.IsNullOrWhiteSpace(texto)
                ? null
                : leer(texto);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static Exception CrearError(string modelo, HttpResponseMessage respuesta, string contenido)
    {
        if (respuesta.IsSuccessStatusCode)
            return new ServicioIANoDisponibleException(MensajeRespuestaInvalida);

        var (codigo, mensaje) = ExtraerError(contenido);
        var detalle = new InvalidOperationException(
            $"La IA no pudo generar el itinerario con el modelo '{modelo}'. "
            + $"Código HTTP: {(int)respuesta.StatusCode}. Detalle: {mensaje}");

        if (codigo == "json_validate_failed")
            return new ServicioIANoDisponibleException(MensajeRespuestaInvalida, detalle);

        return respuesta.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => new ServicioIANoDisponibleException(MensajeCuota, detalle),
            HttpStatusCode.InternalServerError
                or HttpStatusCode.BadGateway
                or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout
                => new ServicioIANoDisponibleException(MensajeSaturado, detalle),
            HttpStatusCode.Unauthorized => new InvalidOperationException(
                "La API key de la IA no es válida.", detalle),
            _ => detalle
        };
    }

    private static (string Codigo, string Mensaje) ExtraerError(string contenido)
    {
        try
        {
            var error = JsonSerializer.Deserialize<RespuestaChat>(contenido, OpcionesJson)?.Error;

            return (error?.Code ?? string.Empty, error?.Message ?? "Respuesta no disponible.");
        }
        catch (JsonException)
        {
            return (string.Empty, "Respuesta no disponible.");
        }
    }

    private static TimeSpan ObtenerEspera(HttpResponseMessage respuesta, int intento)
        => respuesta.Headers.RetryAfter?.Delta is TimeSpan delta && delta > TimeSpan.Zero
            ? delta
            : TimeSpan.FromSeconds(Math.Pow(2, intento));

    private sealed record RespuestaChat(List<OpcionChat>? Choices, ErrorChat? Error);

    private sealed record OpcionChat(MensajeChat? Message);

    private sealed record MensajeChat(string? Content);

    private sealed record ErrorChat(string? Code, string? Message);
}
