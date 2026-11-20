using Itinera.Application.Common.Exceptions;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Facturacion.Dtos;
using Itinera.Application.Facturacion.Interfaces;
using Itinera.Application.Facturacion.Mappers;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;

namespace Itinera.Application.Facturacion.Services;

public sealed class FacturaService(
    IFacturaRepository facturaRepository,
    IPropuestaRepository propuestaRepository,
    IAuditoriaService auditoriaService) : IFacturaService
{
    private readonly IFacturaRepository _facturaRepository = facturaRepository;
    private readonly IPropuestaRepository _propuestaRepository = propuestaRepository;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<FacturaResponse> CrearAsync(CrearFacturaRequest request)
    {
        if (request.Detalles is null || request.Detalles.Count == 0)
            throw new ArgumentException("La factura debe tener al menos un concepto.", nameof(request));

        Propuesta propuesta = await ObtenerPropuestaAsync(request.PropuestaId);

        if (propuesta.Estado != EstadoPropuesta.Aceptada)
            throw new InvalidOperationException(
                "Solo se puede generar una factura para una propuesta aceptada.");

        if (await _facturaRepository.ExisteActivaPorPropuestaAsync(propuesta.Id))
            throw new InvalidOperationException(
                "La propuesta ya posee una factura activa.");

        var fechaVencimiento = FechaUtc(request.FechaVencimiento);

        if (fechaVencimiento.Date < DateTime.UtcNow.Date)
            throw new ArgumentException(
                "La fecha de vencimiento no puede ser anterior a la fecha de emisión.",
                nameof(request));

        var numeroSecuencial = await _facturaRepository
            .ObtenerSiguienteNumeroAsync(propuesta.EmpresaId);
        var factura = new Factura(
            $"FAC-{numeroSecuencial:000000}",
            fechaVencimiento,
            propuesta);

        foreach (var detalle in request.Detalles)
        {
            if (detalle.Cantidad <= 0)
                throw new ArgumentException(
                    "La cantidad de cada concepto debe ser mayor a cero.",
                    nameof(request));
            if (detalle.PrecioUnitario < 0)
                throw new ArgumentException(
                    "El precio unitario no puede ser negativo.",
                    nameof(request));

            factura.AgregarDetalle(new DetalleFactura(
                detalle.Descripcion,
                detalle.Cantidad,
                detalle.PrecioUnitario));
        }

        if (factura.ImporteTotal <= 0)
            throw new ArgumentException(
                "El importe total de la factura debe ser mayor a cero.",
                nameof(request));

        await _facturaRepository.AddAsync(factura);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.CrearFactura,
            nameof(Factura),
            factura.Id);

        return FacturaMapper.ToResponse(factura);
    }

    public async Task<FacturaResponse> ObtenerPorIdAsync(int id)
    {
        var factura = await ObtenerFacturaAsync(id);
        return FacturaMapper.ToResponse(factura);
    }

    public async Task<List<FacturaResponse>> ObtenerTodasAsync()
    {
        var facturas = await _facturaRepository.GetAllAsync();
        return FacturaMapper.ToResponse(facturas);
    }

    public async Task<FacturaResponse> RegistrarPagoAsync(
        int id,
        RegistrarPagoRequest request)
    {
        var factura = await ObtenerFacturaAsync(id);
        if (request.Importe <= 0)
            throw new ArgumentException(
                "El importe del pago debe ser mayor a cero.",
                nameof(request));
        factura.RegistrarPago(new Pago(
            request.Importe,
            FechaUtc(request.Fecha),
            request.MedioPago));

        await _facturaRepository.UpdateAsync(factura);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.RegistrarPagoFactura,
            nameof(Factura),
            factura.Id,
            request.Importe.ToString("F2"));

        return FacturaMapper.ToResponse(factura);
    }

    public async Task AnularAsync(int id)
    {
        var factura = await ObtenerFacturaAsync(id);
        factura.Anular();

        await _facturaRepository.UpdateAsync(factura);
        await _auditoriaService.RegistrarAsync(
            AccionAuditoria.AnularFactura,
            nameof(Factura),
            factura.Id);
    }

    private async Task<Factura> ObtenerFacturaAsync(int id)
    {
        var factura = await _facturaRepository.GetByIdAsync(id);
        return factura ?? throw new FacturaNoEncontradaException(id);
    }

    private async Task<Propuesta> ObtenerPropuestaAsync(int id)
    {
        var propuesta = await _propuestaRepository.GetByIdAsync(id);
        return propuesta ?? throw new PropuestaNoEncontradaException(id);
    }

    private static DateTime FechaUtc(DateTime fecha)
        => DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
}
