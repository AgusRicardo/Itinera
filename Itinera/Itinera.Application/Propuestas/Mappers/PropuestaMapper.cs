using Itinera.Application.Propuestas.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Mappers;

public static class PropuestaMapper
{
    public static PropuestaResponse ToResponse(Propuesta propuesta)
        => new()
        {
            Id = propuesta.Id,
            FechaCreacion = propuesta.FechaCreacion,
            Presupuesto = propuesta.Presupuesto,
            Estado = propuesta.Estado.ToString(),
            ClienteId = propuesta.ClienteId,
            ClienteNombre = NombreCompleto(propuesta.Cliente),
            EmpleadoId = propuesta.EmpleadoId,
            EmpleadoNombre = NombreCompleto(propuesta.Empleado),
            Itinerario = propuesta.Itinerario is null
                ? null
                : ItinerarioMapper.ToResponse(propuesta.Itinerario)
        };

    public static List<PropuestaResponse> ToResponse(IEnumerable<Propuesta> propuestas)
        => propuestas.Select(ToResponse).ToList();

    private static string NombreCompleto(Domain.Common.Persona persona)
        => persona is null ? string.Empty : $"{persona.Nombre} {persona.Apellido}";
}

public static class ItinerarioMapper
{
    public static ItinerarioResponse ToResponse(Itinerario itinerario)
        => new()
        {
            Id = itinerario.Id,
            Nombre = itinerario.Nombre,
            FechaInicio = itinerario.FechaInicio,
            FechaFin = itinerario.FechaFin,
            Destinos = itinerario.Destinos.Select(ToResponse).ToList()
        };

    private static DestinoItinerarioResponse ToResponse(DestinoItinerario destino)
        => new()
        {
            Id = destino.Id,
            Orden = destino.Orden,
            FechaLlegada = destino.FechaLlegada,
            FechaPartida = destino.FechaPartida,
            DestinoId = destino.Destino is null ? 0 : destino.Destino.Id,
            DestinoDescripcion = DescribirDestino(destino.Destino),
            Actividades = destino.ActividadDestinoItinerarios.Select(ToResponse).ToList()
        };

    private static ActividadDestinoItinerarioResponse ToResponse(ActividadDestinoItinerario actividad)
        => new()
        {
            Id = actividad.Id,
            ActividadId = actividad.ActividadId,
            ActividadNombre = actividad.Actividad is null ? string.Empty : actividad.Actividad.Nombre,
            FechaHoraInicio = actividad.FechaHoraInicio,
            CostoFinal = actividad.CostoFinal,
            Observaciones = actividad.Observaciones,
            Orden = actividad.Orden
        };

    private static string DescribirDestino(Destino? destino)
    {
        if (destino?.Ciudad is null)
            return string.Empty;

        var pais = destino.Ciudad.Pais?.Nombre;
        return string.IsNullOrWhiteSpace(pais)
            ? destino.Ciudad.Nombre
            : $"{destino.Ciudad.Nombre}, {pais}";
    }
}
