namespace Itinera.Domain.Common;

public abstract class Persona
{
    public int Id { get; protected set; }
    public string Nombre { get; protected set; }
    public string Apellido { get; protected set; }
    public string Email { get; protected set; }
    public string Telefono { get; protected set; }
    public DateTime FechaRegistracion { get; protected set; }
    public Guid UsuarioRegistracionId { get; protected set; }
    public DateTime? FechaModificacion { get; protected set; }
    public Guid? UsuarioModificacionId { get; protected set; }
    public bool Activo { get; private set; } = true;

    protected Persona(string nombre, string apellido, string email, string telefono)
    {
        Validar(nombre, apellido, email);

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
    }

    public void ActualizarDatos(string nombre, string apellido, string email, string telefono)
    {
        Validar(nombre, apellido, email);

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void Validar(string nombre, string apellido, string email)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido es obligatorio.", nameof(apellido));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.", nameof(email));
    }
}
