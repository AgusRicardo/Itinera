using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Seguridad;

[Authorize(Policy = Permisos.SeguridadGestionar)]
public class RolesController(IRolAdminService rolAdminService) : ApiController
{
    private readonly IRolAdminService _rolAdminService = rolAdminService;

    [HttpGet]
    [ProducesResponseType(typeof(List<RolAdminResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RolAdminResponse>>> Listar()
    {
        var response = await _rolAdminService.ListarAsync();

        return Ok(response);
    }

    [HttpGet("permisos")]
    [ProducesResponseType(typeof(List<PermisoAdminResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PermisoAdminResponse>>> ListarPermisos()
    {
        var response = await _rolAdminService.ListarPermisosAsync();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RolAdminResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RolAdminResponse>> Crear(CrearRolRequest request)
    {
        var response = await _rolAdminService.CrearAsync(request);

        return CreatedAtAction(nameof(Listar), response);
    }

    [HttpPost("{id:int}/permisos")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AsignarPermiso(int id, AsignarPermisoRequest request)
    {
        await _rolAdminService.AsignarPermisoAsync(id, request.PermisoId);

        return NoContent();
    }

    [HttpDelete("{id:int}/permisos/{permisoId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> QuitarPermiso(int id, int permisoId)
    {
        await _rolAdminService.QuitarPermisoAsync(id, permisoId);

        return NoContent();
    }
}
