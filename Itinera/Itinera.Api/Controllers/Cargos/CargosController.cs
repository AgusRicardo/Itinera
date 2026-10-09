using Itinera.Application.Cargos.Dtos;
using Itinera.Application.Cargos.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Cargos;

[Authorize]
public class CargosController(ICargoService cargoService) : ApiController
{
    private readonly ICargoService _cargoService = cargoService;

    [HttpPost]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(CargoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CargoResponse>> Crear(CrearCargoRequest request)
    {
        var response = await _cargoService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(List<CargoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CargoResponse>>> ObtenerTodos()
    {
        var response = await _cargoService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosVer)]
    [ProducesResponseType(typeof(CargoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CargoResponse>> ObtenerPorId(int id)
    {
        var response = await _cargoService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(typeof(CargoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CargoResponse>> Actualizar(int id, ActualizarCargoRequest request)
    {
        var response = await _cargoService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.CatalogosGestionar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _cargoService.EliminarAsync(id);

        return NoContent();
    }
}
