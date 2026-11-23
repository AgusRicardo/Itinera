using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Actividades.Dtos;

public class ActualizarActividadRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal CostoBase { get; set; }

    [Range(1, int.MaxValue)]
    public int DuracionEstimada { get; set; }

    [Range(1, int.MaxValue)]
    public int DestinoId { get; set; }
}
