using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Paises.Dtos;

public class CrearPaisRequest
{
    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}
