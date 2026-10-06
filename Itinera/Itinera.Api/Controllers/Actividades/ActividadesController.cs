using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Actividades.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Actividades;

public class ActividadesController(IActividadService actividadService) : ApiController
{
    private readonly IActividadService _actividadService = actividadService;

    [HttpPost]
    [ProducesResponseType(typeof(ActividadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActividadResponse>> Crear(CrearActividadRequest request)
    {
        var response = await _actividadService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ActividadResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActividadResponse>>> ObtenerTodos()
    {
        var response = await _actividadService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ActividadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActividadResponse>> ObtenerPorId(int id)
    {
        var response = await _actividadService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ActividadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActividadResponse>> Actualizar(int id, ActualizarActividadRequest request)
    {
        var response = await _actividadService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _actividadService.EliminarAsync(id);

        return NoContent();
    }
}
