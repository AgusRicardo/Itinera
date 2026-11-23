using Itinera.Application.Common.Paginacion;
using Itinera.Application.Facturacion.Dtos;
using Itinera.Application.Facturacion.Interfaces;
using Itinera.Api.Services.Facturacion;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Facturacion;

[Authorize]
public sealed class FacturasController(
    IFacturaService facturaService,
    GeneradorPdfFactura generadorPdfFactura) : ApiController
{
    private readonly IFacturaService _facturaService = facturaService;
    private readonly GeneradorPdfFactura _generadorPdfFactura = generadorPdfFactura;

    [HttpGet]
    [Authorize(Policy = Permisos.FacturacionVer)]
    [ProducesResponseType(typeof(List<FacturaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FacturaResponse>>> ObtenerTodas()
        => Ok(await _facturaService.ObtenerTodasAsync());

    [HttpGet("paginado")]
    [Authorize(Policy = Permisos.FacturacionVer)]
    [ProducesResponseType(typeof(Pagina<FacturaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagina<FacturaResponse>>> ObtenerPagina([FromQuery] PaginacionRequest paginacion)
        => Ok(await _facturaService.ObtenerPaginaAsync(paginacion));

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.FacturacionVer)]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FacturaResponse>> ObtenerPorId(int id)
        => Ok(await _facturaService.ObtenerPorIdAsync(id));

    [HttpGet("{id:int}/pdf")]
    [Authorize(Policy = Permisos.FacturacionVer)]
    [Produces("application/pdf")]
    public async Task<IActionResult> DescargarPdf(int id)
    {
        var factura = await _facturaService.ObtenerPorIdAsync(id);
        var contenido = _generadorPdfFactura.Generar(factura);

        return File(contenido, "application/pdf", $"{factura.Numero}.pdf");
    }

    [HttpPost]
    [Authorize(Policy = Permisos.FacturacionCrear)]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaResponse>> Crear(CrearFacturaRequest request)
    {
        var response = await _facturaService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpPost("{id:int}/pagos")]
    [Authorize(Policy = Permisos.FacturacionRegistrarPago)]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaResponse>> RegistrarPago(
        int id,
        RegistrarPagoRequest request)
        => Ok(await _facturaService.RegistrarPagoAsync(id, request));

    [HttpPost("{id:int}/anular")]
    [Authorize(Policy = Permisos.FacturacionAnular)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Anular(int id)
    {
        await _facturaService.AnularAsync(id);
        return NoContent();
    }
}
