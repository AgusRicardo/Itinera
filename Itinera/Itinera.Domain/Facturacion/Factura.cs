using Itinera.Domain.Common;
using Itinera.Domain.Propuestas;

namespace Itinera.Domain.Facturacion;

public class Factura
{
    public int Id { get; private set; }
    public string Numero { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public DateTime FechaVencimiento { get; private set; }
    public decimal ImporteTotal { get; private set; }
    public decimal SaldoPendiente { get; private set; }
    public EstadoFactura Estado { get; private set; }
    public int EstadoFacturaId { get; private set; }
    public int PropuestaId { get; private set; }
    public Propuesta Propuesta { get; private set; }
    public EstadoFacturaCatalogo EstadoFacturaCatalogo { get; private set; }
    public List<DetalleFactura> Detalles { get; private set; } = new();
    public List<Pago> Pagos { get; private set; } = new();

    private Factura()
    {
    }

    public Factura(string numero, DateTime fechaVencimiento, Propuesta propuesta)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de factura es obligatorio.", nameof(numero));

        Numero = numero;
        FechaEmision = DateTime.UtcNow;
        FechaVencimiento = fechaVencimiento;
        Propuesta = propuesta ?? throw new ArgumentNullException(nameof(propuesta));
        PropuestaId = propuesta.Id;
        CambiarEstado(EstadoFactura.Pendiente);
    }

    public void AgregarDetalle(DetalleFactura detalle)
    {
        if (Estado is EstadoFactura.Anulada or EstadoFactura.Pagada)
            throw new InvalidOperationException("La factura no admite nuevos detalles en su estado actual.");

        Detalles.Add(detalle);
        CalcularTotal();
    }

    public void RegistrarPago(Pago pago)
    {
        if (Estado is EstadoFactura.Anulada or EstadoFactura.Pagada)
            throw new InvalidOperationException("La factura no admite nuevos pagos en su estado actual.");

        if (pago.Importe > SaldoPendiente)
            throw new ArgumentException("El importe del pago supera el saldo pendiente.", nameof(pago));

        Pagos.Add(pago);
        SaldoPendiente -= pago.Importe;
        CambiarEstado(SaldoPendiente == 0
            ? EstadoFactura.Pagada
            : EstadoFactura.ParcialmentePagada);
    }

    public void Anular()
    {
        if (Pagos.Count > 0)
            throw new InvalidOperationException("No se puede anular una factura con pagos registrados.");

        if (Estado == EstadoFactura.Pagada)
            throw new InvalidOperationException("No se puede anular una factura pagada.");

        CambiarEstado(EstadoFactura.Anulada);
    }

    private void CalcularTotal()
    {
        ImporteTotal = Detalles.Sum(detalle => detalle.SubTotal);
        SaldoPendiente = ImporteTotal - Pagos.Sum(pago => pago.Importe);
    }

    private void CambiarEstado(EstadoFactura estado)
    {
        Estado = estado;
        EstadoFacturaId = (int)estado;
    }
}
