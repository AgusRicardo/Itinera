using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Facturacion.Dtos;

public sealed class RegistrarPagoRequest
{
    public decimal Importe { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [MaxLength(100)]
    public string MedioPago { get; set; } = string.Empty;
}
