using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Actividades.Dtos;

public class CrearActividadRequest
{
    [Required]
    [MaxLength(150, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;
    [MaxLength(500, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal CostoBase { get; set; }

    [Range(1, int.MaxValue)]
    public int DuracionEstimada { get; set; }

    [Range(1, int.MaxValue)]
    public int DestinoId { get; set; }
}
