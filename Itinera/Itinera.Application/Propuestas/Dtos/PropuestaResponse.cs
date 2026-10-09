namespace Itinera.Application.Propuestas.Dtos;

public class PropuestaResponse
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public decimal Presupuesto { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public ItinerarioResponse? Itinerario { get; set; }
}
