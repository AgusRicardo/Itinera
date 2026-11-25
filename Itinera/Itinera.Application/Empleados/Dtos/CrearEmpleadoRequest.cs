using System.ComponentModel.DataAnnotations;
using Itinera.Application.Common.Validaciones;

namespace Itinera.Application.Empleados.Dtos;

public class CrearEmpleadoRequest
{
    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Apellido { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MaxLength(200, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Email { get; set; } = string.Empty;
    [MaxLength(30, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    [Telefono]
    public string Telefono { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }
}
