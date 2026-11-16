using Itinera.Application.Plataforma.Dtos;
using Itinera.Application.Plataforma.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Administracion;

[Authorize(Policy = Permisos.PlataformaGestionarUsuarios)]
public class AdministracionController(
    IAdministracionPlataformaService administracionPlataformaService) : ApiController
{
    [HttpPost("usuarios")]
    [ProducesResponseType(typeof(UsuarioEmpresaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioEmpresaResponse>> CrearUsuarioEmpresa(
        CrearUsuarioEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        var response = await administracionPlataformaService
            .CrearUsuarioEmpresaAsync(request, cancellationToken);

        return Created($"api/administracion/usuarios/{response.UsuarioId}", response);
    }

    [HttpPut("usuarios/{usuarioId:guid}/empresa")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AsignarUsuarioEmpresa(
        Guid usuarioId,
        AsignarUsuarioEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        await administracionPlataformaService.AsignarUsuarioEmpresaAsync(
            usuarioId,
            request,
            cancellationToken);

        return NoContent();
    }
}
