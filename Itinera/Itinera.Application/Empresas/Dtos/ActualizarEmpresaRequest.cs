namespace Itinera.Application.Empresas.Dtos;

public class ActualizarEmpresaRequest
{
    public string RazonSocial { get; set; } = string.Empty;
    public string CUIT { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
