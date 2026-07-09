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

    protected Persona(string nombre, string apellido, string email, string telefono)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
    }

    public void ActualizarDatos(string nombre, string apellido, string email, string telefono)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
    }
}
