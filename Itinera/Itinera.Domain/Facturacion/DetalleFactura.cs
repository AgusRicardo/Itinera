namespace Itinera.Domain.Facturacion;

public class DetalleFactura
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal SubTotal { get; private set; }
    public int FacturaId { get; private set; }
    public Factura Factura { get; private set; }

    private DetalleFactura()
    {
    }

    public DetalleFactura(string descripcion, decimal cantidad, decimal precioUnitario)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria.", nameof(descripcion));
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        if (precioUnitario < 0)
            throw new ArgumentOutOfRangeException(nameof(precioUnitario));

        Descripcion = descripcion;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        CalcularSubtotal();
    }

    public void CalcularSubtotal() => SubTotal = Cantidad * PrecioUnitario;
}
