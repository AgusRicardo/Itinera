using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Clientes.Dtos;

public class ActualizarClienteRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public string Apellido { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    /// <summary>
    /// Si es <c>null</c> no se modifica el estado actual del cliente.
    /// </summary>
    public bool? Activo { get; set; }
}
