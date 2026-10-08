using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace Itinera.Api.Security;

public sealed class PermisoAuthorizationHandler : AuthorizationHandler<PermisoRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermisoRequirement requirement)
    {
        if (context.User.HasClaim(Permisos.ClaimType, requirement.Permiso))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
