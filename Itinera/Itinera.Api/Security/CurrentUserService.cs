using System.Security.Claims;
using Itinera.Application.Common.Interfaces;

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
}
