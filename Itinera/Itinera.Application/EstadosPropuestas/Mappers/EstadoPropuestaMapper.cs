using Itinera.Application.EstadosPropuestas.Dtos;
using Itinera.Domain.Common;

namespace Itinera.Application.EstadosPropuestas.Mappers;

public static class EstadoPropuestaMapper
{
    public static EstadoPropuestaResponse ToResponse(EstadoPropuestaCatalogo estado) => new()
    {
        Id = estado.Id,
        Descripcion = estado.Descripcion
    };

    public static List<EstadoPropuestaResponse> ToResponse(IEnumerable<EstadoPropuestaCatalogo> estados)
        => estados.Select(ToResponse).ToList();
}
