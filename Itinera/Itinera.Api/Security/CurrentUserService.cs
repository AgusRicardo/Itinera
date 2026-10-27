using System.Security.Claims;
using Itinera.Application.Common.Interfaces;
using Itinera.Security.Application.Common;

namespace Itinera.Api.Security;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UsuarioId
    {
        get
        {
            var valor = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(valor, out var usuarioId) ? usuarioId : null;
        }
    }

    public bool TienePermiso(string codigoPermiso)
    {
        var usuario = httpContextAccessor.HttpContext?.User;
        return usuario?.HasClaim(Permisos.ClaimType, codigoPermiso) ?? false;
    }
}
