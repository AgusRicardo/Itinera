using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Empresas.Dtos;

public class ActualizarEmpresaRequest
{
    [Required]
    [MaxLength(200, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string RazonSocial { get; set; } = string.Empty;

    [Required]
    [MaxLength(20, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string CUIT { get; set; } = string.Empty;

    [MaxLength(30, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    public string Telefono { get; set; } = string.Empty;
}
