namespace Itinera.Domain.Propuestas;

public class Itinerario
{
    public int Id { get; private set; }
    public int PropuestaId { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public string Nombre { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public List<DestinoItinerario> Destinos { get; private set; } = new();
    public DateTime FechaRegistracion { get; protected set; }
    public Guid UsuarioRegistracionId { get; protected set; }
    public DateTime? FechaModificacion { get; protected set; }
    public Guid? UsuarioModificacionId { get; protected set; }

    public Itinerario()
    {
        Destinos = new List<DestinoItinerario>();

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        FechaInicio = hoy;
        FechaFin = hoy;
    }

    public void AgregarDestino(DestinoItinerario destino)
    {
        Destinos.Add(destino);
    }

    public void Modificar()
    {
        // reglas del dominio
    }
}
