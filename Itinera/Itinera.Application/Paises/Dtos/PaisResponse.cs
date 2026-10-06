namespace Itinera.Application.Paises.Dtos;

public class PaisResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
