using Itinera.Security.Aplicacion.Interfaces;
using Itinera.Security.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Seguridad;

public class AuthController(IAuthenticationService authenticationService) : ApiController
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authenticationService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
    {
        var usuarioId = await _authenticationService.RegistrarAsync(request);
        return Ok(new { usuarioId });
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<UsuarioInfoResponse> Me()
    {
        var info = _authenticationService.ObtenerInfoUsuario(User);
        return Ok(info);
    }
}
