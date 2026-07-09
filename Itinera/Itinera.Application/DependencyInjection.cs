using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropuestaService, PropuestaService>();

        return services;
    }
}
