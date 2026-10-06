namespace Itinera.Application.Destinos.Dtos;

public class DestinoResponse
{
    public int Id { get; set; }
    public bool Activo { get; set; }
    public int CiudadId { get; set; }
    public string CiudadNombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public string PaisNombre { get; set; } = string.Empty;
}
