using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Security.Application.Interfaces;
using Itinera.Application.Common.Interfaces;
using Itinera.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Persistence.Repositories.Seguridad;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPropuestaRepository, PropuestaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

        services.AddScoped<ISecurityRepository, SecurityRepository>();

        services.AddPersistence(configuration);

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
        });
        return services;
    }
}
