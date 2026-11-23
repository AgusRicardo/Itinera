namespace Itinera.Application.Propuestas.Dtos;

public sealed class ModificarDiaRequest
{
    public DateOnly Fecha { get; set; }
    public string Instrucciones { get; set; } = string.Empty;
}

public sealed class AplicarModificacionDiaRequest
{
    public AlternativaDiaResponse Alternativa { get; set; } = new();
}

public sealed class AlternativaDiaResponse
{
    public DateOnly Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public List<ActividadAlternativaResponse> Actividades { get; set; } = new();
}
