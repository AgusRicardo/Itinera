using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
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
    public DbSet<EstadoPropuestaCatalogo> EstadosPropuesta => Set<EstadoPropuestaCatalogo>();
    public DbSet<EstadoFacturaCatalogo> EstadosFactura => Set<EstadoFacturaCatalogo>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<DetalleFactura> DetallesFactura => Set<DetalleFactura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<EmpresaCliente> EmpresaClientes => Set<EmpresaCliente>();
    public DbSet<UsuarioComponent> UsuariosSeguridad => Set<UsuarioComponent>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<GrupoUsuarios> GruposUsuarios => Set<GrupoUsuarios>();
    public DbSet<GrupoMiembro> GrupoMiembros => Set<GrupoMiembro>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<UsuarioComponentRol> UsuarioComponentRoles => Set<UsuarioComponentRol>();
    public DbSet<UsuarioEmpleado> UsuariosEmpleados => Set<UsuarioEmpleado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        AplicarFiltrosDeTenant(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void AplicarFiltrosDeTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Propuesta>()
            .HasQueryFilter(p => _tenantContext.EsAdministradorPlataforma || p.EmpresaId == _tenantContext.EmpresaId);

        modelBuilder.Entity<EmpresaCliente>()
            .HasQueryFilter(ec => _tenantContext.EsAdministradorPlataforma || ec.EmpresaId == _tenantContext.EmpresaId);
    }
}
