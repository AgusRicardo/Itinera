using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Plataforma.Dtos;

public sealed class AsignarUsuarioEmpresaRequest
{
    [Range(1, int.MaxValue)]
    public int EmpresaId { get; set; }

    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }

    public string Telefono { get; set; } = string.Empty;
}
