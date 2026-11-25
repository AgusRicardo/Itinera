using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Etiquetas.Dtos;

public sealed class ActualizarEtiquetaRequest
{
    [Required]
    [MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;
}
