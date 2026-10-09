namespace Itinera.Application.Propuestas.Dtos;

public class ModificarItinerarioRequest
{
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public List<DestinoItinerarioRequest> Destinos { get; set; } = new();
}

public class DestinoItinerarioRequest
{
    public int DestinoId { get; set; }
    public int Orden { get; set; }
    public DateOnly FechaLlegada { get; set; }
    public DateOnly FechaPartida { get; set; }
    public List<ActividadDestinoItinerarioRequest> Actividades { get; set; } = new();
}

public class ActividadDestinoItinerarioRequest
{
    public int ActividadId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public decimal CostoFinal { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int Orden { get; set; }
}
