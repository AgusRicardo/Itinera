using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Empleados.Dtos;

public class CrearEmpleadoRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public string Apellido { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }
}
