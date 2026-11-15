using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Itinera.Security.Application.Common;
using Itinera.Security.Application.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Itinera.UnitTests.Seguridad;

public sealed class AuthenticationServiceTests
{
    [Fact]
    public void ObtenerInfoUsuario_lee_claims_nativos_del_jwt()
    {
        var usuarioId = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, "usuario@itinera.test"),
            new Claim(JwtRegisteredClaimNames.Name, "Usuario de prueba"),
            new Claim(ClaimTypes.Role, "Administrador")
        ], "Bearer"));

        var servicio = new AuthenticationService(
            null!,
            Options.Create(new JwtSettings()));

        var respuesta = servicio.ObtenerInfoUsuario(principal);

        Assert.Equal(usuarioId, respuesta.UsuarioId);
        Assert.Equal("usuario@itinera.test", respuesta.Email);
        Assert.Equal("Usuario de prueba", respuesta.Nombre);
        Assert.Contains("Administrador", respuesta.Roles);
    }
}
