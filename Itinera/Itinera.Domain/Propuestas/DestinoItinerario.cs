namespace Itinera.Domain.Propuestas;

public class DestinoItinerario
{
    public int Id { get; private set; }
    public int Orden { get; private set; }
    public DateOnly FechaLlegada { get; private set; }
    public DateOnly FechaPartida { get; private set; }
    public Destino Destino { get; private set; }
    public List<ActividadDestinoItinerario> ActividadDestinoItinerarios { get; private set; } = new();

    private DestinoItinerario()
    {
    }

    public DestinoItinerario(
        int orden,
        DateOnly fechaLlegada,
        DateOnly fechaPartida,
        Destino destino,
        List<ActividadDestinoItinerario> actividadDestinoItinerarios)
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
