using Microsoft.AspNetCore.Authorization;

namespace Itinera.Api.Security;

public sealed class PermisoRequirement(string permiso) : IAuthorizationRequirement
{
    public string Permiso { get; } = permiso;
}
