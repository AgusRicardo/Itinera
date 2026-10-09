namespace Itinera.Application.Propuestas.Dtos;

public class CrearPropuestaRequest
{
    public int ClienteId { get; set; }
    public int EmpleadoId { get; set; }
    public decimal Presupuesto { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Preferencias { get; set; } = string.Empty;
    public List<int> DestinoIds { get; set; } = new();
}
