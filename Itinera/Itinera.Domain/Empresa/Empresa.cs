using Itinera.Domain.Usuarios;

namespace Itinera.Domain.Empresa;

public class Empresa
{
    public int Id { get; private set; }
    public string RazonSocial { get; private set; }
    public string CUIT { get; private set; }
    public string Telefono { get; private set; }
    public DateTime FechaRegistracion { get; protected set; }
    public Guid UsuarioRegistracionId { get; protected set; }
    public DateTime? FechaModificacion { get; protected set; }
    public Guid? UsuarioModificacionId { get; protected set; }
    public bool Activo { get; private set; } = true;
    public List<Empleado> Empleados { get; private set; } = new();

    public Empresa(string razonSocial, string cUIT, string telefono, List<Empleado> empleados)
    {
        RazonSocial = razonSocial;
        CUIT = cUIT;
        Telefono = telefono;
        Empleados = empleados;
    }

    public void CrearEmpleado(Empleado empleado) => Empleados.Add(empleado);
    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
