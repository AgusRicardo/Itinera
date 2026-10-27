using Itinera.Infrastructure.Persistence;

namespace Itinera.Api.Middleware;

public sealed class TenantMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context, TenantResolver tenantResolver)
    {
        tenantResolver.Resolver();
        return next(context);
    }
}
