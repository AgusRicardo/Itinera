namespace Itinera.Domain.Propuestas;

public class ActividadDestinoItinerario
{
    public int Id { get; private set; }
    public int DestinoItinerarioId { get; private set; }
    public int ActividadId { get; private set; }
    public DateTime FechaHoraInicio { get; private set; }
    public decimal CostoFinal { get; private set; }
    public string Observaciones { get; private set; }
    public int Orden { get; private set; }

    public DestinoItinerario DestinoItinerario { get; private set; }
    public Actividad Actividad { get; private set; }

    private ActividadDestinoItinerario()
    {
    }

    public ActividadDestinoItinerario(DateTime fechaHoraInicio, decimal costoFinal, string observaciones, int orden, DestinoItinerario destinoItinerario, Actividad actividad)
    {
        FechaHoraInicio = fechaHoraInicio;
        CostoFinal = costoFinal;
        Observaciones = observaciones;
        Orden = orden;
        DestinoItinerario = destinoItinerario;
        Actividad = actividad;
    }

}
