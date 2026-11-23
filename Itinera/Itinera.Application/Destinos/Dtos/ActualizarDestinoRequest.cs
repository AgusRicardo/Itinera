using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Destinos.Dtos;

public class ActualizarDestinoRequest
{
    [Range(1, int.MaxValue)]
    public int CiudadId { get; set; }
}
