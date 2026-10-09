namespace Itinera.Application.Propuestas.Dtos;

public class DestinoItinerarioResponse
{
    public int Id { get; set; }
    public int Orden { get; set; }
    public DateOnly FechaLlegada { get; set; }
    public DateOnly FechaPartida { get; set; }
    public int DestinoId { get; set; }
    public string DestinoDescripcion { get; set; } = string.Empty;
    public List<ActividadDestinoItinerarioResponse> Actividades { get; set; } = new();
}
