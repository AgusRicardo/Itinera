using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Propuestas.Mappers;

public sealed class PropuestaMapper(
    PerfilIntensidadResolver perfilResolver,
    PlanificacionItinerarioOptions opciones)
{
    public PropuestaResponse ToResponse(Propuesta propuesta)
        => new()
        {
            Id = propuesta.Id,
            FechaCreacion = propuesta.FechaCreacion,
            Presupuesto = propuesta.Presupuesto,
            FechaInicio = propuesta.FechaInicio,
            FechaFin = propuesta.FechaFin,
            CantidadViajeros = propuesta.CantidadViajeros,
            Preferencias = propuesta.Preferencias,
            IntensidadViaje = propuesta.IntensidadViaje,
            Estado = propuesta.Estado.ToString(),
            ClienteId = propuesta.ClienteId,
            ClienteNombre = NombreCompleto(propuesta.Cliente),
            EmpleadoId = propuesta.EmpleadoId,
            EmpleadoNombre = NombreCompleto(propuesta.Empleado),
            Etiquetas = propuesta.Etiquetas
                .Where(item => item.Etiqueta is not null)
                .Select(item => new EtiquetaPropuestaResponse
                {
                    Id = item.EtiquetaId,
                    Nombre = item.Etiqueta.Nombre
                })
                .ToList(),
            Destinos = propuesta.Destinos
                .OrderBy(item => item.Orden)
                .Select(item => ToResponse(propuesta, item))
                .ToList(),
            Actividades = propuesta.Actividades
                .Where(item => item.Actividad is not null)
                .Select(item => new ActividadPropuestaResponse
                {
                    Id = item.ActividadId,
                    DestinoId = item.Actividad.DestinoId,
                    Nombre = item.Actividad.Nombre,
                    CostoBase = item.Actividad.CostoBase
                })
                .ToList(),
            PresupuestoUsado = propuesta.CostoItinerarios,
            PresupuestoDisponible = propuesta.Presupuesto - propuesta.CostoItinerarios,
            ItinerariosCompletos = propuesta.TieneItinerariosCompletos(),
            Limites = CrearLimites(propuesta)
        };

    public List<PropuestaResponse> ToResponse(IEnumerable<Propuesta> propuestas)
        => propuestas.Select(ToResponse).ToList();

    private static DestinoPropuestaResponse ToResponse(Propuesta propuesta, PropuestaDestino destino)
    {
        var itinerario = propuesta.ObtenerItinerario(destino.DestinoId);

        return new DestinoPropuestaResponse
        {
            Id = destino.DestinoId,
            Orden = destino.Orden,
            Descripcion = DescribirDestino(destino.Destino),
            FechaLlegada = destino.FechaLlegada,
            FechaPartida = destino.FechaPartida,
            Faltantes = propuesta.ObtenerFaltantes(destino),
            Itinerario = itinerario is null ? null : ToResponse(propuesta, itinerario)
        };
    }

    private static ItinerarioResponse ToResponse(Propuesta propuesta, Itinerario itinerario)
        => new()
        {
            Id = itinerario.Id,
            Nombre = itinerario.Nombre ?? string.Empty,
            Costo = itinerario.Costo,
            Actividades = itinerario.Actividades
                .Where(item => item.Actividad is not null)
                .OrderBy(item => item.FechaHoraInicio)
                .Select(item => new ActividadItinerarioResponse
                {
                    Id = item.Id,
                    ActividadId = item.ActividadId,
                    Nombre = item.Actividad.Nombre,
                    DuracionEstimada = item.Actividad.DuracionEstimada,
                    FechaHoraInicio = item.FechaHoraInicio,
                    CostoFinal = item.CostoFinal,
                    Observaciones = item.Observaciones ?? string.Empty,
                    PedidaPorCliente = propuesta.Actividades
                        .Any(pedida => pedida.ActividadId == item.ActividadId),
                    GeneradaPorIA = item.RecomendadaPorIA
                })
                .ToList()
        };

    private LimitesIntensidadResponse CrearLimites(Propuesta propuesta)
    {
        var perfil = perfilResolver.Resolver(propuesta.IntensidadViaje);

        return new LimitesIntensidadResponse
        {
            MinutosDisponibles = perfil.MinutosDisponibles,
            MinutosTrasladoEntreActividades = perfil.MinutosTrasladoEntreActividades,
            MinutosPausaDescanso = perfil.MinutosPausaDescanso,
            MinutosTrasladoPorDestino = opciones.MinutosTrasladoPorDestino,
            MaxActividadesPorDia = perfil.MaxActividadesPorDia
        };
    }

    private static string NombreCompleto(Domain.Common.Persona persona)
        => persona is null ? string.Empty : $"{persona.Nombre} {persona.Apellido}";

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
