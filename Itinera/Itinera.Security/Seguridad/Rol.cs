namespace Itinera.Security.Domain;

public class Rol
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public List<Permiso> Permisos { get; private set; } = new();

    public Rol(string nombre, string descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    private Rol() { }
}
