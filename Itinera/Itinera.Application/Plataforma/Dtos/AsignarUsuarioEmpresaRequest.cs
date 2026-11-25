using System.ComponentModel.DataAnnotations;
using Itinera.Application.Common.Validaciones;

namespace Itinera.Application.Plataforma.Dtos;

public sealed class AsignarUsuarioEmpresaRequest
{
    [Range(1, int.MaxValue)]
    public int EmpresaId { get; set; }

    [Range(1, int.MaxValue)]
    public int CargoId { get; set; }

    [MaxLength(30, ErrorMessage = "Este campo admite hasta {1} caracteres.")]
    [Telefono]
    public string Telefono { get; set; } = string.Empty;
}
