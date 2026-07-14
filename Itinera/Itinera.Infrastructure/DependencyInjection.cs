using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Seguridad.Interfaces;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Persistence.Repositories.Seguridad;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPropuestaRepository, PropuestaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

        services.AddScoped<ISecurityRepository, SecurityRepository>();

        return services;
    }
}
