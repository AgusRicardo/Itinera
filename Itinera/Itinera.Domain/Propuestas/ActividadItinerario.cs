namespace Itinera.Domain.Propuestas;

public class ActividadItinerario
{
    public int Id { get; private set; }
    public int ItinerarioId { get; private set; }
    public int ActividadId { get; private set; }
    public DateTime FechaHoraInicio { get; private set; }
    public decimal CostoFinal { get; private set; }
    public string Observaciones { get; private set; }
    public int Orden { get; private set; }
    public bool RecomendadaPorIA { get; private set; }

    public Itinerario Itinerario { get; private set; }
    public Actividad Actividad { get; private set; }

    public DateTime FechaHoraFin => FechaHoraInicio.AddMinutes(Actividad.DuracionEstimada);
    public DateOnly Fecha => DateOnly.FromDateTime(FechaHoraInicio);

    private ActividadItinerario()
    {
    }

    public ActividadItinerario(
        Actividad actividad,
        DateTime fechaHoraInicio,
        int cantidadViajeros,
        string observaciones,
        int orden,
        bool recomendadaPorIA = false)
    {
        RecomendadaPorIA = recomendadaPorIA;
        Actividad = actividad ?? throw new ArgumentNullException(nameof(actividad));
        ActividadId = actividad.Id;
        FechaHoraInicio = fechaHoraInicio;
        Observaciones = observaciones ?? string.Empty;
        Orden = orden;
        RecalcularCosto(cantidadViajeros);
    }

    public void RecalcularCosto(int cantidadViajeros)
    {
        if (Actividad is not null)
            CostoFinal = Actividad.CostoBase * cantidadViajeros;
    }
}
