using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Propuestas.Dtos;

public class CrearPropuestaRequest
{
    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue)]
    public int EmpleadoId { get; set; }

    [Range(1, double.MaxValue)]
    public decimal Presupuesto { get; set; }

    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Preferencias { get; set; } = string.Empty;

    [MinLength(1)]
    public List<int> DestinoIds { get; set; } = new();

    public List<int> EtiquetaIds { get; set; } = new();
}
