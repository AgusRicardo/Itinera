namespace Itinera.Application.Conceptos.Dtos;

public sealed class ConceptoFacturaResponse
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public decimal? Porcentaje { get; set; }
}
