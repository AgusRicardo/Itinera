using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Cargos.Dtos;

public class CrearCargoRequest
{
    [Required]
    public string Descripcion { get; set; } = string.Empty;
}
