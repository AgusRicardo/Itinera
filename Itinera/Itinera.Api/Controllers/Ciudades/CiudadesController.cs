using Itinera.Application.Ciudades.Dtos;
using Itinera.Application.Ciudades.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Ciudades;

public class CiudadesController(ICiudadService ciudadService) : ApiController
{
    private readonly ICiudadService _ciudadService = ciudadService;

    [HttpPost]
    [ProducesResponseType(typeof(CiudadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CiudadResponse>> Crear(CrearCiudadRequest request)
    {
        var response = await _ciudadService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CiudadResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CiudadResponse>>> ObtenerTodos()
    {
        var response = await _ciudadService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CiudadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CiudadResponse>> ObtenerPorId(int id)
    {
        var response = await _ciudadService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CiudadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CiudadResponse>> Actualizar(int id, ActualizarCiudadRequest request)
    {
        var response = await _ciudadService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _ciudadService.EliminarAsync(id);

        return NoContent();
    }
}
