using System.ComponentModel.DataAnnotations;
using Itinera.Domain.Common;

namespace Itinera.Application.Propuestas.Dtos;

public abstract class ConfiguracionPropuestaRequest
{
    [Range(1, double.MaxValue)]
    public decimal Presupuesto { get; set; }

    [Range(1, int.MaxValue)]
    public int CantidadViajeros { get; set; } = 1;

    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }

    [MaxLength(2000)]
    public string Preferencias { get; set; } = string.Empty;

    public IntensidadViaje IntensidadViaje { get; set; } = IntensidadViaje.Media;

    [MinLength(1)]
    public List<EstadiaRequest> Destinos { get; set; } = new();

    public List<int> ActividadIds { get; set; } = new();

    public List<int> EtiquetaIds { get; set; } = new();
}

public sealed class EstadiaRequest
{
    [Range(1, int.MaxValue)]
    public int DestinoId { get; set; }

    public DateOnly FechaLlegada { get; set; }
    public DateOnly FechaPartida { get; set; }
}

public sealed class ActualizarPropuestaRequest : ConfiguracionPropuestaRequest
{
}
