namespace Itinera.Domain.Facturacion;

public class DetalleFactura
{
    public int Id { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal? Porcentaje { get; private set; }
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

    public static DetalleFactura PorPorcentaje(string descripcion, decimal porcentaje, decimal costoDelViaje)
    {
        if (porcentaje is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(
                nameof(porcentaje), "El porcentaje debe ser mayor a 0 y hasta 100.");
        if (costoDelViaje <= 0)
            throw new InvalidOperationException(
                "La propuesta no tiene costo de itinerarios para calcular el porcentaje.");

        var importe = Math.Round(costoDelViaje * porcentaje / 100, 2, MidpointRounding.AwayFromZero);

        return new DetalleFactura(descripcion, 1, importe) { Porcentaje = porcentaje };
    }

    public void CalcularSubtotal() => SubTotal = Cantidad * PrecioUnitario;
}
