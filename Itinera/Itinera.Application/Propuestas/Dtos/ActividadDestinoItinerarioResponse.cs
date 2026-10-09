namespace Itinera.Application.Propuestas.Dtos;

public class ActividadDestinoItinerarioResponse
{
    public int Id { get; set; }
    public int ActividadId { get; set; }
    public string ActividadNombre { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public decimal CostoFinal { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int Orden { get; set; }
}
