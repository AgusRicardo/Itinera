namespace Itinera.Application.Actividades.Dtos;

public class ActividadResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoBase { get; set; }
    public int DuracionEstimada { get; set; }
    public bool Activo { get; set; }
    public int DestinoId { get; set; }
    public int CiudadId { get; set; }
    public string CiudadNombre { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public string PaisNombre { get; set; } = string.Empty;
}
