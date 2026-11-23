using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Paises.Dtos;

public class ActualizarPaisRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
}
