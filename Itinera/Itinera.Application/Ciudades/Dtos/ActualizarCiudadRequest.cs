using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Ciudades.Dtos;

public class ActualizarCiudadRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PaisId { get; set; }
}
