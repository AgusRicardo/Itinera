namespace Itinera.Application.Propuestas.Dtos;

public class ItinerarioResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Costo { get; set; }
    public List<ActividadItinerarioResponse> Actividades { get; set; } = new();
}

public sealed class ActividadItinerarioResponse
{
    public int Id { get; set; }
    public int ActividadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DuracionEstimada { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public decimal CostoFinal { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public bool PedidaPorCliente { get; set; }
    public bool GeneradaPorIA { get; set; }
}
