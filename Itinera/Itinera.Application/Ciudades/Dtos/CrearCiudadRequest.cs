namespace Itinera.Application.Ciudades.Dtos;

public class CrearCiudadRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
}
