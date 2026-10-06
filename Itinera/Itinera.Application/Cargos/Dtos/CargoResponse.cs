namespace Itinera.Application.Cargos.Dtos;

public class CargoResponse
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
