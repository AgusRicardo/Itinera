using Itinera.Application.Paises.Dtos;
using Itinera.Application.Paises.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Paises;

[Authorize]
public class PaisesController(IPaisService paisService) : ApiController
{
    private readonly IPaisService _paisService = paisService;

    [HttpPost]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(PaisResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaisResponse>> Crear(CrearPaisRequest request)
    {
        var response = await _paisService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(List<PaisResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaisResponse>>> ObtenerTodos()
    {
        var response = await _paisService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(PaisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaisResponse>> ObtenerPorId(int id)
    {
        var response = await _paisService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(PaisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaisResponse>> Actualizar(int id, ActualizarPaisRequest request)
    {
        var response = await _paisService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _paisService.EliminarAsync(id);

        return NoContent();
    }
}
