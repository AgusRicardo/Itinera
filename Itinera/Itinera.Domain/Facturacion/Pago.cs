namespace Itinera.Domain.Facturacion;

public class Pago
{
    public int Id { get; private set; }
    public DateTime Fecha { get; private set; }
    public decimal Importe { get; private set; }
    public string MedioPago { get; private set; }
    public int FacturaId { get; private set; }
    public Factura Factura { get; private set; }

    private Pago()
    {
    }

    public Pago(decimal importe, DateTime fecha, string medioPago)
    {
        if (importe <= 0)
            throw new ArgumentOutOfRangeException(nameof(importe));
        if (string.IsNullOrWhiteSpace(medioPago))
            throw new ArgumentException("El medio de pago es obligatorio.", nameof(medioPago));

        Importe = importe;
        Fecha = fecha;
        MedioPago = medioPago;
    }
}
