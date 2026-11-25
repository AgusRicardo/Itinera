using Itinera.Application.Facturacion.Dtos;
using Itinera.Domain.Facturacion;

namespace Itinera.Application.Facturacion.Mappers;

public static class FacturaMapper
{
    public static FacturaResponse ToResponse(Factura factura)
        => new()
        {
            Id = factura.Id,
            Numero = factura.Numero,
            FechaEmision = factura.FechaEmision,
            FechaVencimiento = factura.FechaVencimiento,
            ImporteTotal = factura.ImporteTotal,
            SaldoPendiente = factura.SaldoPendiente,
            Estado = factura.Estado.ToString(),
            PropuestaId = factura.PropuestaId,
            EmpresaId = factura.EmpresaId,
            EmpresaNombre = factura.Propuesta.Empleado?.Empresa?.RazonSocial ?? string.Empty,
            ClienteNombre = factura.Propuesta.Cliente is null
                ? string.Empty
                : $"{factura.Propuesta.Cliente.Nombre} {factura.Propuesta.Cliente.Apellido}".Trim(),
            Detalles = factura.Detalles.Select(detalle => new DetalleFacturaResponse
            {
                Id = detalle.Id,
                Descripcion = detalle.Descripcion,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Porcentaje = detalle.Porcentaje,
                SubTotal = detalle.SubTotal
            }).ToList(),
            Pagos = factura.Pagos.Select(pago => new PagoResponse
            {
                Id = pago.Id,
                Fecha = pago.Fecha,
                Importe = pago.Importe,
                MedioPago = pago.MedioPago
            }).ToList()
        };

    public static List<FacturaResponse> ToResponse(IEnumerable<Factura> facturas)
        => facturas.Select(ToResponse).ToList();
}
