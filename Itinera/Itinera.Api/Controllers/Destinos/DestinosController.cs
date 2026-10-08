using Itinera.Application.Destinos.Dtos;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Destinos;

[Authorize]
public class DestinosController(IDestinoService destinoService) : ApiController
{
    private readonly IDestinoService _destinoService = destinoService;

    [HttpPost]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(DestinoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DestinoResponse>> Crear(CrearDestinoRequest request)
    {
        var response = await _destinoService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(List<DestinoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DestinoResponse>>> ObtenerTodos()
    {
        var response = await _destinoService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(DestinoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DestinoResponse>> ObtenerPorId(int id)
    {
        var response = await _destinoService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(DestinoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DestinoResponse>> Actualizar(int id, ActualizarDestinoRequest request)
    {
        var response = await _destinoService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _destinoService.EliminarAsync(id);

        return NoContent();
    }
}
