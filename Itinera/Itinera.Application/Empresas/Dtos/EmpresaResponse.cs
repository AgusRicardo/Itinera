namespace Itinera.Application.Empresas.Dtos;

public class EmpresaResponse
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string CUIT { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
