using Itinera.Application.Etiquetas.Dtos;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Etiquetas;

[Authorize(Policy = Permisos.EtiquetasVer)]
public sealed class EtiquetasController(IEtiquetaService etiquetaService) : ApiController
{
    private readonly IEtiquetaService _etiquetaService = etiquetaService;

    [HttpGet]
    public async Task<ActionResult<List<EtiquetaResponse>>> ObtenerTodos()
        => Ok(await _etiquetaService.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EtiquetaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EtiquetaResponse>> ObtenerPorId(int id)
        => Ok(await _etiquetaService.ObtenerPorIdAsync(id));

    [HttpPost]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<ActionResult<EtiquetaResponse>> Crear(CrearEtiquetaRequest request)
    {
        var response = await _etiquetaService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<ActionResult<EtiquetaResponse>> Actualizar(
        int id,
        ActualizarEtiquetaRequest request)
        => Ok(await _etiquetaService.ActualizarAsync(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _etiquetaService.EliminarAsync(id);
        return NoContent();
    }
}
