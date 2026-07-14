using Itinera.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace Itinera.Infrastructure.Persistence;

public class SecurityDbContext : DbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsuarioComponent> UsuariosSeguridad => Set<UsuarioComponent>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<GrupoUsuarios> GruposUsuarios => Set<GrupoUsuarios>();
    public DbSet<GrupoMiembro> GrupoMiembros => Set<GrupoMiembro>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<UsuarioComponentRol> UsuarioComponentRoles => Set<UsuarioComponentRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecurityDbContext).Assembly,
            t => t.Namespace?.Contains("Seguridad") == true);

        base.OnModelCreating(modelBuilder);
    }
}
