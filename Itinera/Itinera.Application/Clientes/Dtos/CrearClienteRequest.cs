using System.ComponentModel.DataAnnotations;
using Itinera.Application.Common.Validaciones;

namespace Itinera.Application.Clientes.Dtos;

public class CrearClienteRequest
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

    /// <summary>
    /// Si es <c>false</c> el cliente queda creado desactivado. Por defecto queda activo.
    /// </summary>
    public bool? Activo { get; set; }
}
