using Itinera.Application.Empresas.Dtos;
using Itinera.Application.Empresas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Empresas;

public class EmpresasController(IEmpresaService empresaService) : ApiController
{
    private readonly IEmpresaService _empresaService = empresaService;

    [HttpPost]
    [ProducesResponseType(typeof(EmpresaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmpresaResponse>> Crear(CrearEmpresaRequest request)
    {
        var response = await _empresaService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<EmpresaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmpresaResponse>>> ObtenerTodos()
    {
        var response = await _empresaService.ObtenerTodosAsync();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmpresaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpresaResponse>> ObtenerPorId(int id)
    {
        var response = await _empresaService.ObtenerPorIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmpresaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpresaResponse>> Actualizar(int id, ActualizarEmpresaRequest request)
    {
        var response = await _empresaService.ActualizarAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _empresaService.EliminarAsync(id);

        return NoContent();
    }
}
