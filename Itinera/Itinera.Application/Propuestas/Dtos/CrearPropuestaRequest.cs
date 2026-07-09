namespace Itinera.Application.Propuestas.Dtos;

public class CrearPropuestaRequest
{
    public int ClienteId { get; set; }
    public int EmpleadoId { get; set; }
    public decimal Presupuesto { get; set; }
}
