using Itinera.Application.Ciudades.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Ciudades.Mappers;

public static class CiudadMapper
{
    public static CiudadResponse ToResponse(Ciudad ciudad) => new()
    {
        Id = ciudad.Id,
        Nombre = ciudad.Nombre,
        Activo = ciudad.Activo,
        PaisId = ciudad.PaisId,
        PaisNombre = ciudad.Pais?.Nombre ?? string.Empty
    };

    public static List<CiudadResponse> ToResponse(IEnumerable<Ciudad> ciudades)
        => ciudades.Select(ToResponse).ToList();
}
