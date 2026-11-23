using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Models;

namespace Itinera.Application.Propuestas.Planificacion;

public static class ItinerarioGeneradoMapper
{
    public static AlternativaItinerarioResponse ToResponse(
        ItinerarioGenerado generado,
        SolicitudItinerario solicitud)
    {
        var pedidas = solicitud.Destinos
            .SelectMany(destino => destino.Actividades)
            .Select(actividad => actividad.ActividadId)
            .ToHashSet();

        return new AlternativaItinerarioResponse
        {
            Nombre = generado.Nombre ?? string.Empty,
            Descripcion = generado.Descripcion ?? string.Empty,
            CostoEstimado = generado.CostoEstimado,
            Actividades = generado.Destinos
                .SelectMany(destino => destino.Actividades)
                .OrderBy(actividad => actividad.FechaHoraInicio)
                .Select((actividad, indice) => new ActividadAlternativaResponse
                {
                    ActividadId = actividad.ActividadId,
                    Nombre = actividad.Nombre,
                    Descripcion = actividad.Descripcion,
                    CostoFinal = actividad.CostoFinal,
                    DuracionEstimada = actividad.DuracionEstimada,
                    FechaHoraInicio = actividad.FechaHoraInicio,
                    Observaciones = actividad.Observaciones,
                    Orden = indice + 1,
                    PedidaPorCliente = actividad.ActividadId is int id && pedidas.Contains(id)
                })
                .ToList()
        };
    }
}
