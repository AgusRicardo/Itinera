namespace Itinera.Application.Empleados.Dtos;

public class CrearEmpleadoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int CargoId { get; set; }
    public int EmpresaId { get; set; }
}
