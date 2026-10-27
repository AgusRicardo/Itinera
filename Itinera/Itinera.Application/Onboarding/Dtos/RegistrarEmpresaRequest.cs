namespace Itinera.Application.Onboarding.Dtos;

public class RegistrarEmpresaRequest
{
    public string RazonSocial { get; set; } = string.Empty;
    public string CUIT { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string AdministradorNombre { get; set; } = string.Empty;
    public string AdministradorApellido { get; set; } = string.Empty;
    public string AdministradorEmail { get; set; } = string.Empty;
    public string AdministradorPassword { get; set; } = string.Empty;
}
