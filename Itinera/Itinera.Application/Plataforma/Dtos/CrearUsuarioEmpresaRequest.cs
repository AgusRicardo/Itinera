using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Plataforma.Dtos;

public sealed class CrearUsuarioEmpresaRequest
{
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;
    public int CargoId { get; set; }
    public int RolId { get; set; }
}
