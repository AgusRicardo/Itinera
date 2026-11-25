using System.Data;
using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Catalogos;
using Itinera.Domain.Empresa;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Itinera.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public const string FiltroEmpresa = "Empresa";
    public const string FiltroPersonaActiva = "PersonaActiva";
    public const string FiltroCatalogoActivo = "CatalogoActivo";
    public const string FiltroEmpresaActiva = "EmpresaActiva";
    public const string FiltroNoEliminada = "NoEliminada";

    private static readonly StringTypeMapping TipoTexto = new("text", DbType.String);
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    internal int? EmpresaActualId => _tenantContext.EmpresaId;

    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Propuesta> Propuestas => Set<Propuesta>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<Itinerario> Itinerarios => Set<Itinerario>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<ActividadItinerario> ActividadesItinerario => Set<ActividadItinerario>();
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<EstadoPropuestaCatalogo> EstadosPropuesta => Set<EstadoPropuestaCatalogo>();
    public DbSet<EstadoFacturaCatalogo> EstadosFactura => Set<EstadoFacturaCatalogo>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<DetalleFactura> DetallesFactura => Set<DetalleFactura>();
    public DbSet<ConceptoFactura> ConceptosFactura => Set<ConceptoFactura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();
    public DbSet<PropuestaDestino> PropuestasDestinos => Set<PropuestaDestino>();
    public DbSet<PropuestaActividad> PropuestasActividades => Set<PropuestaActividad>();
    public DbSet<PropuestaEtiqueta> PropuestasEtiquetas => Set<PropuestaEtiqueta>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();
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
        MapearSinTildes(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>En la base: minúsculas y sin tildes, igual que <c>TextoBusqueda.Normalizar</c>.</summary>
    public static string SinTildes(string texto) => throw new NotSupportedException();

    private static void MapearSinTildes(ModelBuilder modelBuilder)
        => modelBuilder
            .HasDbFunction(typeof(AppDbContext).GetMethod(nameof(SinTildes))!)
            .HasTranslation(argumentos => new SqlFunctionExpression(
                "translate",
                [
                    new SqlFunctionExpression(
                        "lower",
                        [argumentos[0]],
                        nullable: true,
                        argumentsPropagateNullability: [true],
                        typeof(string),
                        TipoTexto),
                    new SqlFragmentExpression("'áàäâãåéèëêíìïîóòöôõúùüûñç'"),
                    new SqlFragmentExpression("'aaaaaaeeeeiiiiooooouuuunc'")
                ],
                nullable: true,
                argumentsPropagateNullability: [true, false, false],
                typeof(string),
                TipoTexto));

    private void AplicarFiltrosDeTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Propuesta>()
            .HasQueryFilter(
                FiltroEmpresa,
                p => _tenantContext.EsAdministradorPlataforma || p.EmpresaId == _tenantContext.EmpresaId)
            .HasQueryFilter(
                FiltroNoEliminada,
                p => _tenantContext.EsAdministradorPlataforma
                    || p.EstadoPropuestaId != (int)EstadoPropuesta.Eliminada);

        modelBuilder.Entity<Factura>()
            .HasQueryFilter(f =>
                _tenantContext.EsAdministradorPlataforma
                || (_tenantContext.EmpresaId.HasValue
                    && f.EmpresaId == _tenantContext.EmpresaId.Value));

        modelBuilder.Entity<Etiqueta>()
            .HasQueryFilter(etiqueta =>
                _tenantContext.EsAdministradorPlataforma
                || (_tenantContext.EmpresaId.HasValue
                    && etiqueta.EmpresaId == _tenantContext.EmpresaId.Value));

        modelBuilder.Entity<ConceptoFactura>()
            .HasQueryFilter(concepto => concepto.EmpresaId == _tenantContext.EmpresaId);

        modelBuilder.Entity<EmpresaCliente>()
            .HasQueryFilter(ec => _tenantContext.EsAdministradorPlataforma || ec.EmpresaId == _tenantContext.EmpresaId);
    }
}
