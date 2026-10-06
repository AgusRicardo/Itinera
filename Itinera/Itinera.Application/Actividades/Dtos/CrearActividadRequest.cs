namespace Itinera.Application.Actividades.Dtos;

public class CrearActividadRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoBase { get; set; }
    public int DuracionEstimada { get; set; }
    public int DestinoId { get; set; }
}
