namespace Itinera.Application.Ciudades.Dtos;

public class CiudadResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int PaisId { get; set; }
    public string PaisNombre { get; set; } = string.Empty;
}
