namespace Itinera.Security.Domain;

public class Permiso
{
    public int Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descripcion { get; private set; }
    public List<Rol> Roles { get; private set; } = new();

    public Permiso(string codigo, string descripcion)
    {
        Codigo = codigo;
        Descripcion = descripcion;
    }

    private Permiso() { }
}
