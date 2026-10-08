using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Seguridad;

[Authorize(Policy = Permisos.SeguridadGestionar)]
public class UsuariosController(IUsuarioAdminService usuarioAdminService) : ApiController
{
    private readonly IUsuarioAdminService _usuarioAdminService = usuarioAdminService;

    [HttpGet]
    [ProducesResponseType(typeof(List<UsuarioAdminResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UsuarioAdminResponse>>> Listar()
    {
        var response = await _usuarioAdminService.ListarAsync();

        return Ok(response);
    }

    [HttpPost("{id:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AsignarRol(Guid id, AsignarRolRequest request)
    {
        await _usuarioAdminService.AsignarRolAsync(id, request.RolId);

        return NoContent();
    }

    [HttpDelete("{id:guid}/roles/{rolId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> QuitarRol(Guid id, int rolId)
    {
        await _usuarioAdminService.QuitarRolAsync(id, rolId);

        return NoContent();
    }

    [HttpPost("{id:guid}/activar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activar(Guid id)
    {
        await _usuarioAdminService.ActivarAsync(id);

        return NoContent();
    }

    [HttpPost("{id:guid}/desactivar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desactivar(Guid id)
    {
        await _usuarioAdminService.DesactivarAsync(id);

        return NoContent();
    }
}
