using Itinera.Application.Empleados.Dtos;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Empleados;

[Authorize]
public class EmpleadosController(IEmpleadoService empleadoService) : ApiController
{
    private readonly IEmpleadoService _empleadoService = empleadoService;

    [HttpPost]
    [Authorize(Policy = Permisos.EmpleadosGestionar)]
    [ProducesResponseType(typeof(EmpleadoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpleadoResponse>> Crear(CrearEmpleadoRequest request)
    {
        var response = await _empleadoService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [Authorize(Policy = Permisos.EmpleadosVer)]
    [ProducesResponseType(typeof(List<EmpleadoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmpleadoResponse>>> ObtenerTodos()
    {
        var response = await _empleadoService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permisos.EmpleadosVer)]
    [ProducesResponseType(typeof(EmpleadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpleadoResponse>> ObtenerPorId(int id)
    {
        var response = await _empleadoService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.EmpleadosGestionar)]
    [ProducesResponseType(typeof(EmpleadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpleadoResponse>> Actualizar(int id, ActualizarEmpleadoRequest request)
    {
        var response = await _empleadoService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.EmpleadosGestionar)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _empleadoService.EliminarAsync(id);

        return NoContent();
    }
}
