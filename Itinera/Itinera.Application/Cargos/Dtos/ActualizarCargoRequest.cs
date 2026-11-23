using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Cargos.Dtos;

public class ActualizarCargoRequest
{
    [Required]
    public string Descripcion { get; set; } = string.Empty;
}
