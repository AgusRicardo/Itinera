namespace Itinera.Infrastructure.Servicios.IA;

public sealed class GeneradorIaOptions
{
    public const string SectionName = "InteligenciaArtificial";

    public string UrlBase { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public List<string> ModelosAlternativos { get; set; } = new();
    public string EsfuerzoRazonamiento { get; set; } = string.Empty;
    public int MaxTokensRespuesta { get; set; } = 5500;
    public int CantidadAlternativas { get; set; } = 3;
    public decimal Temperatura { get; set; } = 0.2m;
    public int TimeoutSegundos { get; set; } = 60;
}
