using Itinera.Application.Actividades.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Actividades.Mappers;

public static class ActividadMapper
{
    public static ActividadResponse ToResponse(Actividad actividad) => new()
    {
        Id = actividad.Id,
        Nombre = actividad.Nombre,
        Descripcion = actividad.Descripcion,
        CostoBase = actividad.CostoBase,
        DuracionEstimada = actividad.DuracionEstimada,
        Activo = actividad.Activo,
        DestinoId = actividad.DestinoId,
        CiudadId = actividad.Destino?.CiudadId ?? 0,
        CiudadNombre = actividad.Destino?.Ciudad?.Nombre ?? string.Empty,
        PaisId = actividad.Destino?.Ciudad?.PaisId ?? 0,
        PaisNombre = actividad.Destino?.Ciudad?.Pais?.Nombre ?? string.Empty
    };

    public static List<ActividadResponse> ToResponse(IEnumerable<Actividad> actividades)
        => actividades.Select(ToResponse).ToList();
}
