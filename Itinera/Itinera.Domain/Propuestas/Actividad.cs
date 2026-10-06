namespace Itinera.Domain.Propuestas;

public class Actividad
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal CostoBase { get; private set; }
    public int DuracionEstimada { get; private set; }
    public int DestinoId { get; private set; }
    public bool Activo { get; private set; } = true;
    public Destino Destino { get; private set; }

    private Actividad()
    {
    }

    public Actividad(
        string nombre,
        string descripcion,
        decimal costoBase,
        int duracionEstimada,
        Destino destino)
    {
        Validar(nombre, costoBase, duracionEstimada);
        ArgumentNullException.ThrowIfNull(destino);

        Nombre = nombre;
        Descripcion = descripcion;
        CostoBase = costoBase;
        DuracionEstimada = duracionEstimada;
        Destino = destino;
        DestinoId = destino.Id;
    }

    public void ActualizarInformacion(
        string nombre,
        string descripcion,
        decimal costoBase,
        int duracionEstimada,
        Destino destino)
    {
        Validar(nombre, costoBase, duracionEstimada);
        ArgumentNullException.ThrowIfNull(destino);

        Nombre = nombre;
        Descripcion = descripcion;
        CostoBase = costoBase;
        DuracionEstimada = duracionEstimada;
        Destino = destino;
        DestinoId = destino.Id;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void Validar(string nombre, decimal costoBase, int duracionEstimada)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la actividad es obligatorio.", nameof(nombre));

        if (costoBase < 0)
            throw new ArgumentOutOfRangeException(nameof(costoBase), "El costo base no puede ser negativo.");

        if (duracionEstimada <= 0)
            throw new ArgumentOutOfRangeException(nameof(duracionEstimada), "La duración estimada debe ser mayor a cero.");
    }
}
