using Itinera.Application.Cargos.Dtos;
using Itinera.Application.Cargos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Cargos;

public class CargosController(ICargoService cargoService) : ApiController
{
    private readonly ICargoService _cargoService = cargoService;

    [HttpPost]
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
    [ProducesResponseType(typeof(List<CargoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CargoResponse>>> ObtenerTodos()
    {
        var response = await _cargoService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CargoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CargoResponse>> ObtenerPorId(int id)
    {
        var response = await _cargoService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CargoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CargoResponse>> Actualizar(int id, ActualizarCargoRequest request)
    {
        var response = await _cargoService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _cargoService.EliminarAsync(id);

        return NoContent();
    }
}
