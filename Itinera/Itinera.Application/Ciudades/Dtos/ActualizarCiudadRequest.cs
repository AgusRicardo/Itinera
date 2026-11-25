using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Ciudades.Dtos;

public class ActualizarCiudadRequest
{
    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PaisId { get; set; }
}
