using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Plataforma.Dtos;

public sealed class CrearUsuarioEmpresaRequest
{
    [Range(1, int.MaxValue)]
    public int EmpresaId { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }

    [Range(1, int.MaxValue)]
    public int RolId { get; set; }
}
