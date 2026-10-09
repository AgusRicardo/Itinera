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

    public void DefinirNombre(string nombre)
    {
        Nombre = nombre;
    }

    public void DefinirFechas(DateOnly fechaInicio, DateOnly fechaFin)
    {
        if (fechaFin < fechaInicio)
            throw new ArgumentException(
                "La fecha de fin no puede ser anterior a la fecha de inicio.", nameof(fechaFin));

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public void AgregarDestino(DestinoItinerario destino)
    {
        Destinos.Add(destino);
    }

    public void ReemplazarDestinos(IEnumerable<DestinoItinerario> destinos)
    {
        Destinos.Clear();
        Destinos.AddRange(destinos);
    }
}
