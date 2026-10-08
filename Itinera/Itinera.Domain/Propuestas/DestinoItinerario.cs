namespace Itinera.Domain.Propuestas;

public class DestinoItinerario
{
    public int Id { get; private set; }
    public int Orden { get; private set; }
    public DateTime FechaLlegada { get; private set; }
    public DateTime FechaPartida { get; private set; }
    public Destino Destino { get; private set; }
    public List<ActividadDestinoItinerario> ActividadDestinoItinerarios { get; private set; } = new();

    private DestinoItinerario()
    {
    }

    public DestinoItinerario(int orden, DateTime fechaLlegada, DateTime fechaPartida, Destino destino, List<ActividadDestinoItinerario> actividadDestinoItinerarios)
    {
        Orden = orden;
        FechaLlegada = fechaLlegada;
        FechaPartida = fechaPartida;
        Destino = destino;
        ActividadDestinoItinerarios = actividadDestinoItinerarios;
    }

    public void AgregarActividad(ActividadDestinoItinerario actividadDestinoItinerario)
    {
        ActividadDestinoItinerarios.Add(actividadDestinoItinerario);
    }
}
