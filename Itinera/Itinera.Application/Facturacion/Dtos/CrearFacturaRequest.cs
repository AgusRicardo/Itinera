using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Facturacion.Dtos;

public sealed class CrearFacturaRequest
{
    [Range(1, int.MaxValue)]
    public int PropuestaId { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }

    [Required]
    [MinLength(1)]
    public List<CrearDetalleFacturaRequest> Detalles { get; set; } = [];
}

public sealed class CrearDetalleFacturaRequest
{
    [Range(1, int.MaxValue)]
    public int ConceptoId { get; set; }

    public decimal Cantidad { get; set; }
}
