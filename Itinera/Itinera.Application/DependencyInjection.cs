using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Services;
using Itinera.Application.Seguridad.Common;
using Itinera.Application.Seguridad.Interfaces;
using Itinera.Application.Seguridad.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropuestaService, PropuestaService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }

    public static IServiceCollection AddJwtSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        return services;
    }
}
