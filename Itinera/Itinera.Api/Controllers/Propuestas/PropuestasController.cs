using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Propuestas;

[Authorize(Policy = Permisos.PropuestasVer)]
public class PropuestasController(
    IPropuestaService propuestaService,
    IGeneradorItinerariosService generadorItinerariosService)
    : PaginadoController<PropuestaResponse, ListadoComercialRequest>(propuestaService)
{
    private readonly IPropuestaService _propuestaService = propuestaService;
    private readonly IGeneradorItinerariosService _generadorItinerariosService = generadorItinerariosService;

    [HttpPost]
    [Authorize(Policy = Permisos.PropuestasCrear)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropuestaResponse>> Crear(CrearPropuestaRequest request)
    {
        var response = await _propuestaService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet("resumen")]
    [ProducesResponseType(typeof(ResumenPropuestasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResumenPropuestasResponse>> ObtenerResumen()
        => Ok(await _propuestaService.ObtenerResumenAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropuestaResponse>> ObtenerPorId(int id)
    {
        var response = await _propuestaService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PropuestaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PropuestaResponse>>> ObtenerTodos(
        [FromQuery] int? clienteId)
    {
        var response = clienteId.HasValue
            ? await _propuestaService.ObtenerPorClienteAsync(clienteId.Value)
            : await _propuestaService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.PropuestasEditar)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> Actualizar(int id, ActualizarPropuestaRequest request)
        => Ok(await _propuestaService.ActualizarAsync(id, request));

    [HttpPut("{id:int}/destinos/{destinoId:int}/itinerario")]
    [Authorize(Policy = Permisos.ItinerariosEditar)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> GuardarItinerario(
        int id,
        int destinoId,
        GuardarItinerarioRequest request)
        => Ok(await _propuestaService.GuardarItinerarioAsync(
            id,
            destinoId,
            request));

    [HttpPost("{id:int}/destinos/{destinoId:int}/itinerario/alternativas")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [ProducesResponseType(typeof(List<AlternativaItinerarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<List<AlternativaItinerarioResponse>>> GenerarAlternativas(
        int id,
        int destinoId,
        GenerarAlternativasRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.GenerarAlternativasAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/destinos/{destinoId:int}/itinerario/aplicar-alternativa")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> AplicarAlternativa(
        int id,
        int destinoId,
        AplicarAlternativaRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.AplicarAlternativaAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/destinos/{destinoId:int}/itinerario/dia/alternativas")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [ProducesResponseType(typeof(List<AlternativaDiaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<List<AlternativaDiaResponse>>> GenerarAlternativasDia(
        int id,
        int destinoId,
        ModificarDiaRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.GenerarAlternativasDiaAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/destinos/{destinoId:int}/itinerario/dia/aplicar-alternativa")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> AplicarAlternativaDia(
        int id,
        int destinoId,
        AplicarModificacionDiaRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.AplicarAlternativaDiaAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/destinos/{destinoId:int}/cambiar-destino/alternativas")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [ProducesResponseType(typeof(List<AlternativaItinerarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<List<AlternativaItinerarioResponse>>> GenerarCambioDestino(
        int id,
        int destinoId,
        GenerarCambioDestinoRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.GenerarCambioDestinoAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/destinos/{destinoId:int}/cambiar-destino")]
    [Authorize(Policy = Permisos.ItinerariosGenerarIa)]
    [Authorize(Policy = Permisos.PropuestasEditar)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> CambiarDestino(
        int id,
        int destinoId,
        CambiarDestinoRequest request,
        CancellationToken cancellationToken)
        => Ok(await _generadorItinerariosService.CambiarDestinoAsync(
            id,
            destinoId,
            request,
            cancellationToken));

    [HttpPost("{id:int}/presentar")]
    [Authorize(Policy = Permisos.PropuestasPresentar)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> Presentar(int id)
    {
        var response = await _propuestaService.PresentarAsync(id);

        return Ok(response);
    }

    [HttpPost("{id:int}/respuesta")]
    [Authorize(Policy = Permisos.PropuestasRegistrarRespuesta)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> RegistrarRespuesta(
        int id,
        RegistrarRespuestaRequest request)
    {
        var response = await _propuestaService.RegistrarRespuestaAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.PropuestasEliminar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _propuestaService.EliminarAsync(id);

        return NoContent();
    }
}
