using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Empresas.Dtos;

public class CrearEmpresaRequest
{
    [Required]
    public string RazonSocial { get; set; } = string.Empty;

    [Required]
    public string CUIT { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;
}
