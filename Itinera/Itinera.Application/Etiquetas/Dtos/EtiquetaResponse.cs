namespace Itinera.Application.Etiquetas.Dtos;

public sealed class EtiquetaResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public bool Activo { get; set; }
}
