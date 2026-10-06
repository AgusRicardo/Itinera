using System.Net;
using Itinera.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no manejada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, detail) = exception switch
        {
            UnauthorizedAccessException ex =>
                (HttpStatusCode.Unauthorized, ex.Message),
            NotFoundException ex =>
                (HttpStatusCode.NotFound, ex.Message),
            InvalidOperationException ex =>
                (HttpStatusCode.Conflict, ex.Message),
            KeyNotFoundException ex =>
                (HttpStatusCode.NotFound, ex.Message),
            ArgumentException ex =>
                (HttpStatusCode.BadRequest, ex.Message),
            _ =>
                (HttpStatusCode.InternalServerError, "Error interno del servidor.")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = statusCode switch
            {
                HttpStatusCode.Unauthorized => "No autorizado",
                HttpStatusCode.Conflict => "Conflicto",
                HttpStatusCode.NotFound => "No encontrado",
                HttpStatusCode.BadRequest => "Solicitud inválida",
                _ => "Error interno del servidor"
            },
            Detail = detail,
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
