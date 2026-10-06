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

    private Empresa()
    {
    }

    public Empresa(string razonSocial, string cuit, string telefono)
    {
        Validar(razonSocial, cuit);

        RazonSocial = razonSocial;
        CUIT = cuit;
        Telefono = telefono;
    }

    public void ActualizarDatos(string razonSocial, string cuit, string telefono)
    {
        Validar(razonSocial, cuit);

        RazonSocial = razonSocial;
        CUIT = cuit;
        Telefono = telefono;
    }

    public void CrearEmpleado(Empleado empleado) => Empleados.Add(empleado);
    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void Validar(string razonSocial, string cuit)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));

        if (string.IsNullOrWhiteSpace(cuit))
            throw new ArgumentException("El CUIT es obligatorio.", nameof(cuit));
    }
}
