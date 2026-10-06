using Itinera.Application.EstadosFacturas.Dtos;
using Itinera.Domain.Common;

namespace Itinera.Application.EstadosFacturas.Mappers;

public static class EstadoFacturaMapper
{
    public static EstadoFacturaResponse ToResponse(EstadoFacturaCatalogo estado) => new()
    {
        Id = estado.Id,
        Descripcion = estado.Descripcion
    };

    public static List<EstadoFacturaResponse> ToResponse(IEnumerable<EstadoFacturaCatalogo> estados)
        => estados.Select(ToResponse).ToList();
}
