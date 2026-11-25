namespace Itinera.Application.Facturacion.Dtos;

public sealed class FacturaResponse
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal ImporteTotal { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int PropuestaId { get; set; }
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public List<DetalleFacturaResponse> Detalles { get; set; } = [];
    public List<PagoResponse> Pagos { get; set; } = [];
}

public sealed class DetalleFacturaResponse
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? Porcentaje { get; set; }
    public decimal SubTotal { get; set; }
}

public sealed class PagoResponse
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = string.Empty;
}
