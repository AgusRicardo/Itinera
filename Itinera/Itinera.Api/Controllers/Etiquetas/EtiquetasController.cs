using Itinera.Application.Etiquetas.Dtos;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Etiquetas;

[Authorize]
public sealed class EtiquetasController(IEtiquetaService etiquetaService) : ApiController
{
    private readonly IEtiquetaService _etiquetaService = etiquetaService;

    [HttpGet]
    [Authorize(Policy = Permisos.EtiquetasVer)]
    public async Task<ActionResult<List<EtiquetaResponse>>> ObtenerTodas()
        => Ok(await _etiquetaService.ObtenerTodasAsync());

    [HttpPost]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<ActionResult<EtiquetaResponse>> Crear(CrearEtiquetaRequest request)
        => Ok(await _etiquetaService.CrearAsync(request));

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<ActionResult<EtiquetaResponse>> Actualizar(
        int id,
        CrearEtiquetaRequest request)
        => Ok(await _etiquetaService.ActualizarAsync(id, request));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permisos.EtiquetasGestionar)]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _etiquetaService.DesactivarAsync(id);
        return NoContent();
    }
}
