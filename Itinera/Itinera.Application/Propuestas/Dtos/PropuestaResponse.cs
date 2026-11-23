using Itinera.Domain.Common;

namespace Itinera.Application.Propuestas.Dtos;

public class PropuestaResponse
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public decimal Presupuesto { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public int CantidadViajeros { get; set; }
    public string Preferencias { get; set; } = string.Empty;
    public IntensidadViaje IntensidadViaje { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public List<EtiquetaPropuestaResponse> Etiquetas { get; set; } = new();
    public List<DestinoPropuestaResponse> Destinos { get; set; } = new();
    public List<ActividadPropuestaResponse> Actividades { get; set; } = new();
    public decimal PresupuestoUsado { get; set; }
    public decimal PresupuestoDisponible { get; set; }
    public bool ItinerariosCompletos { get; set; }
    public LimitesIntensidadResponse Limites { get; set; } = new();
}

public sealed class DestinoPropuestaResponse
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly FechaLlegada { get; set; }
    public DateOnly FechaPartida { get; set; }
    public ItinerarioResponse? Itinerario { get; set; }
    public List<string> Faltantes { get; set; } = new();
}

public sealed class LimitesIntensidadResponse
{
    public int MinutosDisponibles { get; set; }
    public int MinutosTrasladoEntreActividades { get; set; }
    public int MinutosPausaDescanso { get; set; }
    public int MinutosTrasladoPorDestino { get; set; }
    public int MaxActividadesPorDia { get; set; }
}

public sealed class ActividadPropuestaResponse
{
    public int Id { get; set; }
    public int DestinoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoBase { get; set; }
}

public sealed class EtiquetaPropuestaResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
