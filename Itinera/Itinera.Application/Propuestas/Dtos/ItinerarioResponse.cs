namespace Itinera.Application.Propuestas.Dtos;

public class ItinerarioResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public List<DestinoItinerarioResponse> Destinos { get; set; } = new();
}
