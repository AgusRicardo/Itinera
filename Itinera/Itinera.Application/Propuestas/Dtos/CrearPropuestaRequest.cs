using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Propuestas.Dtos;

public class CrearPropuestaRequest : ConfiguracionPropuestaRequest
{
    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue)]
    public int EmpleadoId { get; set; }
}
