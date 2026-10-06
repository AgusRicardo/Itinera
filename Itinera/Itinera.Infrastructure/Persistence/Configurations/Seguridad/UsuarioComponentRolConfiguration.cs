using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class UsuarioComponentRolConfiguration : IEntityTypeConfiguration<UsuarioComponentRol>
{
    public void Configure(EntityTypeBuilder<UsuarioComponentRol> builder)
    {
        builder.ToTable("UsuarioComponentRoles");

        builder.HasKey(x => new { x.UsuarioComponentId, x.RolId });

        builder.HasOne(x => x.UsuarioComponent)
               .WithMany()
               .HasForeignKey(x => x.UsuarioComponentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Rol)
               .WithMany()
               .HasForeignKey(x => x.RolId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
