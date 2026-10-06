namespace Itinera.Application.Empleados.Dtos;

public class EmpleadoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int CargoId { get; set; }
    public string CargoDescripcion { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public string EmpresaRazonSocial { get; set; } = string.Empty;
}
