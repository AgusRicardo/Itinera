using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Application.EstadosFacturas.Interfaces;
using Itinera.Application.Facturacion.Interfaces;
using Itinera.Application.Etiquetas.Interfaces;
using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Application.Onboarding.Interfaces;
using Itinera.Application.Plataforma.Interfaces;
using Itinera.Application.Paises.Interfaces;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Security.Application.Interfaces;
using Itinera.Application.Common.Interfaces;
using Itinera.Infrastructure.Persistence;
using Itinera.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.Configuration;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Persistence.Repositories.Seguridad;
using Itinera.Infrastructure.Servicios;
using Itinera.Infrastructure.Servicios.IA;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPropuestaRepository, PropuestaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IEmpresaClienteRepository, EmpresaClienteRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<ICiudadRepository, CiudadRepository>();
        services.AddScoped<IDestinoRepository, DestinoRepository>();
        services.AddScoped<IActividadRepository, ActividadRepository>();
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IEstadoPropuestaRepository, EstadoPropuestaRepository>();
        services.AddScoped<IEstadoFacturaRepository, EstadoFacturaRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IEtiquetaRepository, EtiquetaRepository>();

        services.AddScoped<ISecurityRepository, SecurityRepository>();

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<TenantResolver>();

        var generadorIaOptions = new GeneradorIaOptions();
        configuration.GetSection(GeneradorIaOptions.SectionName).Bind(generadorIaOptions);
        services.AddSingleton(generadorIaOptions);

        if (string.IsNullOrWhiteSpace(generadorIaOptions.UrlBase)
            || string.IsNullOrWhiteSpace(generadorIaOptions.Modelo)
            || string.IsNullOrWhiteSpace(generadorIaOptions.ApiKey))
            throw new InvalidOperationException(
                "Deben configurarse 'UrlBase', 'Modelo' y 'ApiKey' de 'InteligenciaArtificial'.");

        services.AddSingleton(new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(generadorIaOptions.TimeoutSegundos)
        });
        services.AddScoped<IGeneradorItinerarioIA, GroqGeneradorItinerarioIA>();

        var planificacion = new PlanificacionItinerarioOptions();
        configuration.GetSection(PlanificacionItinerarioOptions.SectionName).Bind(planificacion);
        services.AddSingleton(planificacion);
        services.AddSingleton<PerfilIntensidadResolver>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IAdministracionPlataformaService, AdministracionPlataformaService>();

        services.AddScoped<SecuritySeeder>();

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
            var proveedor = configuration["Database:Provider"] ?? "PostgreSQL";

            if (!string.Equals(proveedor, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Proveedor de base de datos no soportado: {proveedor}.");
            }

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
        });
        return services;
    }
}
