namespace Itinera.Domain.Propuestas;

public class Pais
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public bool Activo { get; private set; } = true;
    public ICollection<Ciudad> Ciudades { get; private set; } = new List<Ciudad>();

    private Pais()
    {
    }

    public Pais(string nombre)
    {
        ValidarNombre(nombre);
        Nombre = nombre;
    }

    public void ActualizarNombre(string nombre)
    {
        ValidarNombre(nombre);
        Nombre = nombre;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del país es obligatorio.", nameof(nombre));
    }
}
