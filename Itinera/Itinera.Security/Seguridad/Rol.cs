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

    public void AgregarPermiso(Permiso permiso)
    {
        ArgumentNullException.ThrowIfNull(permiso);

        if (!Permisos.Any(p => p.Codigo == permiso.Codigo))
            Permisos.Add(permiso);
    }

    public void QuitarPermiso(Permiso permiso)
    {
        ArgumentNullException.ThrowIfNull(permiso);

        var existente = Permisos.FirstOrDefault(p => p.Codigo == permiso.Codigo);
        if (existente is not null)
            Permisos.Remove(existente);
    }
}
