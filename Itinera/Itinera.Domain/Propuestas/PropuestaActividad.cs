namespace Itinera.Domain.Propuestas;

public class PropuestaActividad
{
    public int PropuestaId { get; private set; }
    public int ActividadId { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public Actividad Actividad { get; private set; }

    private PropuestaActividad()
    {
    }

    public PropuestaActividad(Propuesta propuesta, Actividad actividad)
    {
        Propuesta = propuesta ?? throw new ArgumentNullException(nameof(propuesta));
        Actividad = actividad ?? throw new ArgumentNullException(nameof(actividad));
        PropuestaId = propuesta.Id;
        ActividadId = actividad.Id;
    }
}
