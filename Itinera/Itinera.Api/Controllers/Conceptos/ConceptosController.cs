using Itinera.Application.Common.Paginacion;
using Itinera.Application.Conceptos.Dtos;
using Itinera.Application.Conceptos.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Conceptos;

[Authorize(Policy = Permisos.ConceptosVer)]
public sealed class ConceptosController(IConceptoFacturaService conceptoService)
    : PaginadoController<ConceptoFacturaResponse, ListadoRequest>(conceptoService)
{
    private readonly IConceptoFacturaService _conceptoService = conceptoService;

    [HttpGet]
    public async Task<ActionResult<List<ConceptoFacturaResponse>>> ObtenerTodos()
        => Ok(await _conceptoService.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ConceptoFacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConceptoFacturaResponse>> ObtenerPorId(int id)
        => Ok(await _conceptoService.ObtenerPorIdAsync(id));

    [HttpPost]
    [Authorize(Policy = Permisos.ConceptosGestionar)]
    public async Task<ActionResult<ConceptoFacturaResponse>> Crear(CrearConceptoFacturaRequest request)
    {
        var response = await _conceptoService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.ConceptosGestionar)]
    public async Task<ActionResult<ConceptoFacturaResponse>> Actualizar(int id, ActualizarConceptoFacturaRequest request)
        => Ok(await _conceptoService.ActualizarAsync(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.ConceptosGestionar)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _conceptoService.EliminarAsync(id);
        return NoContent();
    }
}
