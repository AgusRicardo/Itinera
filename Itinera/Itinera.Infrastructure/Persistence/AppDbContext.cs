using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Propuesta> Propuestas => Set<Propuesta>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<Itinerario> Itinerarios => Set<Itinerario>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<DestinoItinerario> DestinoItinerarios => Set<DestinoItinerario>();
    public DbSet<ActividadDestinoItinerario> ActividadesDestinoItinerario => Set<ActividadDestinoItinerario>();
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly,
            t => t.Namespace?.Contains("Seguridad") != true);

        base.OnModelCreating(modelBuilder);
    }
}
