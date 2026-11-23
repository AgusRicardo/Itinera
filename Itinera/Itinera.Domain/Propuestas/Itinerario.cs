namespace Itinera.Domain.Propuestas;

public class Itinerario
{
    public int Id { get; private set; }
    public int PropuestaId { get; private set; }
    public int DestinoId { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public Destino Destino { get; private set; }
    public string Nombre { get; private set; }
    public List<ActividadItinerario> Actividades { get; private set; } = new();
    public DateTime FechaRegistracion { get; protected set; }
    public Guid UsuarioRegistracionId { get; protected set; }
    public DateTime? FechaModificacion { get; protected set; }
    public Guid? UsuarioModificacionId { get; protected set; }

    private Itinerario()
    {
    }

    public Itinerario(Destino destino)
    {
        Destino = destino ?? throw new ArgumentNullException(nameof(destino));
        DestinoId = destino.Id;
    }

    public decimal Costo => Actividades.Sum(actividad => actividad.CostoFinal);

    public bool IncluyeActividad(int actividadId)
        => Actividades.Any(actividad => actividad.ActividadId == actividadId);

    public void DefinirNombre(string nombre) => Nombre = nombre;

    public void ReemplazarActividades(IEnumerable<ActividadItinerario> actividades)
    {
        var nuevas = actividades.ToList();

        if (nuevas.Count == 0)
            throw new ArgumentException("El itinerario debe tener al menos una actividad.", nameof(actividades));

        if (nuevas.Any(item => item.Actividad.DestinoId != DestinoId))
            throw new ArgumentException("Una actividad no pertenece al destino del itinerario.", nameof(actividades));

        Actividades.Clear();
        Actividades.AddRange(nuevas);
    }
}
