namespace Itinera.Domain.Propuestas;

public class Ciudad
{
    public int Id { get; private set; }
    public int PaisId { get; private set; }
    public string Nombre { get; private set; }
    public bool Activo { get; private set; } = true;

    public Pais Pais { get; private set; }
    public ICollection<Destino> Destinos { get; private set; } = new List<Destino>();

    private Ciudad()
    {
    }

    public Ciudad(string nombre, Pais pais)
    {
        ValidarNombre(nombre);
        ArgumentNullException.ThrowIfNull(pais);

        Nombre = nombre;
        Pais = pais;
        PaisId = pais.Id;
    }

    public void ActualizarDatos(string nombre, Pais pais)
    {
        ValidarNombre(nombre);
        ArgumentNullException.ThrowIfNull(pais);

        Nombre = nombre;
        Pais = pais;
        PaisId = pais.Id;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ciudad es obligatorio.", nameof(nombre));
    }
}
