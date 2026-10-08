using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Seguridad;

[Authorize(Policy = Permisos.SeguridadGestionar)]
public class GruposController(IGrupoAdminService grupoAdminService) : ApiController
{
    private readonly IGrupoAdminService _grupoAdminService = grupoAdminService;

    [HttpGet]
    [ProducesResponseType(typeof(List<GrupoAdminResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GrupoAdminResponse>>> Listar()
    {
        var response = await _grupoAdminService.ListarAsync();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GrupoAdminResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GrupoAdminResponse>> Crear(CrearGrupoRequest request)
    {
        var response = await _grupoAdminService.CrearAsync(request);

        return CreatedAtAction(nameof(Listar), response);
    }

    [HttpPost("{id:guid}/miembros")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgregarMiembro(Guid id, AgregarMiembroRequest request)
    {
        await _grupoAdminService.AgregarMiembroAsync(id, request.MiembroId);

        return NoContent();
    }

    [HttpDelete("{id:guid}/miembros/{miembroId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> QuitarMiembro(Guid id, Guid miembroId)
    {
        await _grupoAdminService.QuitarMiembroAsync(id, miembroId);

        return NoContent();
    }

    [HttpPost("{id:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AsignarRol(Guid id, AsignarRolRequest request)
    {
        await _grupoAdminService.AsignarRolAsync(id, request.RolId);

        return NoContent();
    }

    [HttpDelete("{id:guid}/roles/{rolId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> QuitarRol(Guid id, int rolId)
    {
        await _grupoAdminService.QuitarRolAsync(id, rolId);

        return NoContent();
    }
}
