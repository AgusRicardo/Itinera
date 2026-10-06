using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Actividades.Services;
using Itinera.Application.Cargos.Interfaces;
using Itinera.Application.Cargos.Services;
using Itinera.Application.Ciudades.Interfaces;
using Itinera.Application.Ciudades.Services;
using Itinera.Application.Clientes.Interfaces;
using Itinera.Application.Clientes.Services;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Destinos.Services;
using Itinera.Application.Empleados.Interfaces;
using Itinera.Application.Empleados.Services;
using Itinera.Application.Empresas.Interfaces;
using Itinera.Application.Empresas.Services;
using Itinera.Application.EstadosFacturas.Interfaces;
using Itinera.Application.EstadosFacturas.Services;
using Itinera.Application.EstadosPropuestas.Interfaces;
using Itinera.Application.EstadosPropuestas.Services;
using Itinera.Application.Paises.Interfaces;
using Itinera.Application.Paises.Services;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Itinera.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropuestaService, PropuestaService>();
        services.AddScoped<IPaisService, PaisService>();
        services.AddScoped<ICiudadService, CiudadService>();
        services.AddScoped<IDestinoService, DestinoService>();
        services.AddScoped<IActividadService, ActividadService>();
        services.AddScoped<ICargoService, CargoService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IEstadoPropuestaService, EstadoPropuestaService>();
        services.AddScoped<IEstadoFacturaService, EstadoFacturaService>();

        return services;
    }

}
