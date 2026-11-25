using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Onboarding.Dtos;

public class RegistrarEmpresaRequest
{
    [Required]
    [MaxLength(200, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string RazonSocial { get; set; } = string.Empty;

    [Required]
    [MaxLength(20, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string CUIT { get; set; } = string.Empty;

    [MaxLength(30, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string AdministradorNombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string AdministradorApellido { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string AdministradorEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string AdministradorPassword { get; set; } = string.Empty;
}
