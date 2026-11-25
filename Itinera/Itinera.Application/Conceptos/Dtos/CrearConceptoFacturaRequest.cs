using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Conceptos.Dtos;

public sealed class CrearConceptoFacturaRequest
{
    [Required]
    [MaxLength(300)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, 999_999_999)]
    public decimal Precio { get; set; }

    [Range(0.01, 100)]
    public decimal? Porcentaje { get; set; }
}
