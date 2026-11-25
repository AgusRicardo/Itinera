using Itinera.Application.Conceptos.Dtos;
using Itinera.Domain.Facturacion;

namespace Itinera.Application.Conceptos.Mappers;

public static class ConceptoFacturaMapper
{
    public static ConceptoFacturaResponse ToResponse(ConceptoFactura concepto) => new()
    {
        Id = concepto.Id,
        Descripcion = concepto.Descripcion,
        Precio = concepto.Precio,
        Porcentaje = concepto.Porcentaje
    };

    public static List<ConceptoFacturaResponse> ToResponse(IEnumerable<ConceptoFactura> conceptos)
        => conceptos.Select(ToResponse).ToList();
}
