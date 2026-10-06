using Itinera.Application.Destinos.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Destinos.Mappers;

public static class DestinoMapper
{
    public static DestinoResponse ToResponse(Destino destino) => new()
    {
        Id = destino.Id,
        Activo = destino.Activo,
        CiudadId = destino.CiudadId,
        CiudadNombre = destino.Ciudad?.Nombre ?? string.Empty,
        PaisId = destino.Ciudad?.PaisId ?? 0,
        PaisNombre = destino.Ciudad?.Pais?.Nombre ?? string.Empty
    };

    public static List<DestinoResponse> ToResponse(IEnumerable<Destino> destinos)
        => destinos.Select(ToResponse).ToList();
}
