using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Paises.Dtos;

public class CrearPaisRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
}
