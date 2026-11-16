namespace Itinera.Application.Plataforma.Dtos;

public sealed class AsignarUsuarioEmpresaRequest
{
    public int EmpresaId { get; set; }
    public int CargoId { get; set; }
    public string Telefono { get; set; } = string.Empty;
}
