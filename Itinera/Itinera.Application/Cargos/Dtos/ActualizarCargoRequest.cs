using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Cargos.Dtos;

public class ActualizarCargoRequest
{
    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}
