namespace Itinera.Application.Ciudades.Dtos;

public class ActualizarCiudadRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
}
