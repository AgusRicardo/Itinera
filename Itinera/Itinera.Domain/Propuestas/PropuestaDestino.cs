namespace Itinera.Domain.Propuestas;

public class PropuestaDestino
{
    public int PropuestaId { get; private set; }
    public int DestinoId { get; private set; }
    public int Orden { get; private set; }
    public DateOnly FechaLlegada { get; private set; }
    public DateOnly FechaPartida { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public Destino Destino { get; private set; }

    private PropuestaDestino()
    {
    }

    public PropuestaDestino(Propuesta propuesta, Estadia estadia, int orden)
    {
        Propuesta = propuesta ?? throw new ArgumentNullException(nameof(propuesta));
        Destino = estadia.Destino ?? throw new ArgumentNullException(nameof(estadia));
        PropuestaId = propuesta.Id;
        DestinoId = estadia.Destino.Id;
        DefinirEstadia(estadia.FechaLlegada, estadia.FechaPartida, orden);
    }

    public void DefinirEstadia(DateOnly fechaLlegada, DateOnly fechaPartida, int orden)
    {
        FechaLlegada = fechaLlegada;
        FechaPartida = fechaPartida;
        Orden = orden;
    }

    public int Dias => FechaPartida.DayNumber - FechaLlegada.DayNumber + 1;

    public bool Incluye(DateOnly fecha) => fecha >= FechaLlegada && fecha <= FechaPartida;
}
