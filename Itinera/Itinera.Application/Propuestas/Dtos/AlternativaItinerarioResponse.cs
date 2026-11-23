namespace Itinera.Application.Propuestas.Dtos;

public sealed class AlternativaItinerarioResponse
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public List<ActividadAlternativaResponse> Actividades { get; set; } = new();
}

public sealed class ActividadAlternativaResponse
{
    public int? ActividadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoFinal { get; set; }
    public int DuracionEstimada { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool PedidaPorCliente { get; set; }
}

public sealed class AplicarAlternativaRequest
{
    public AlternativaItinerarioResponse Alternativa { get; set; } = new();
}
