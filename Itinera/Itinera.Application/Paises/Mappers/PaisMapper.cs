using Itinera.Application.Paises.Dtos;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Paises.Mappers;

public static class PaisMapper
{
    public static PaisResponse ToResponse(Pais pais) => new()
    {
        Id = pais.Id,
        Nombre = pais.Nombre,
        Activo = pais.Activo
    };

    public static List<PaisResponse> ToResponse(IEnumerable<Pais> paises)
        => paises.Select(ToResponse).ToList();
}
