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
    [Required]
    [MaxLength(300)]
    public string Descripcion { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }
}
