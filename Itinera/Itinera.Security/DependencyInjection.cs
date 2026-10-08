using Itinera.Security.Application.Common;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}
