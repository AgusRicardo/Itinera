using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Propuestas;

[Authorize]
public class PropuestasController(IPropuestaService propuestaService) : ApiController
{
    private readonly IPropuestaService _propuestaService = propuestaService;

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

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.PropuestasVer)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropuestaResponse>> ObtenerPorId(int id)
    {
        var response = await _propuestaService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Policy = Permisos.PropuestasVer)]
    [ProducesResponseType(typeof(List<PropuestaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PropuestaResponse>>> ObtenerPorCliente([FromQuery] int clienteId)
    {
        var response = await _propuestaService.ObtenerPorClienteAsync(clienteId);

        return Ok(response);
    }

    [HttpGet("{id:int}/itinerario")]
    [Authorize(Policy = Permisos.PropuestasVer)]
    [ProducesResponseType(typeof(ItinerarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItinerarioResponse>> ObtenerItinerario(int id)
    {
        var response = await _propuestaService.ObtenerItinerarioAsync(id);

        return Ok(response);
    }

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

    [HttpPut("{id:int}/itinerario")]
    [Authorize(Policy = Permisos.ItinerariosEditar)]
    [ProducesResponseType(typeof(PropuestaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropuestaResponse>> ModificarItinerario(
        int id,
        ModificarItinerarioRequest request)
    {
        var response = await _propuestaService.ModificarItinerarioAsync(id, request);

        return Ok(response);
    }
}
