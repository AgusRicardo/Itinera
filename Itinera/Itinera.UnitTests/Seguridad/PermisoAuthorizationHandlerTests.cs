using System.Security.Claims;
using Itinera.Api.Security;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Itinera.UnitTests.Seguridad;

public class PermisoAuthorizationHandlerTests
{
    [Fact]
    public async Task ConcedeCuandoElUsuarioTieneElPermiso()
    {
        var contexto = CrearContexto(Permisos.ClientesVer, Permisos.ClientesVer);
        var handler = new PermisoAuthorizationHandler();

        await handler.HandleAsync(contexto);

        Assert.True(contexto.HasSucceeded);
    }

    [Fact]
    public async Task NoConcedeCuandoElUsuarioNoTieneElPermiso()
    {
        var contexto = CrearContexto(Permisos.ClientesVer, Permisos.SeguridadGestionar);
        var handler = new PermisoAuthorizationHandler();

        await handler.HandleAsync(contexto);

        Assert.False(contexto.HasSucceeded);
    }

    private static AuthorizationHandlerContext CrearContexto(
        string permisoDelUsuario,
        string permisoRequerido)
    {
        var identidad = new ClaimsIdentity(
            new[] { new Claim(Permisos.ClaimType, permisoDelUsuario) },
            authenticationType: "Test");
        var usuario = new ClaimsPrincipal(identidad);
        var requerimiento = new PermisoRequirement(permisoRequerido);

        return new AuthorizationHandlerContext(
            new[] { requerimiento },
            usuario,
            resource: null);
    }
}
