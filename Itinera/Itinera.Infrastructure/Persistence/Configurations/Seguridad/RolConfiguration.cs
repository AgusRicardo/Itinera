using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Nombre)
               .IsUnique();

        builder.Property(x => x.Descripcion)
               .HasMaxLength(300);

        builder.HasMany(x => x.Permisos)
               .WithMany(x => x.Roles)
               .UsingEntity("RolPermiso",
                   l => l.HasOne(typeof(Permiso)).WithMany().HasForeignKey("PermisoId"),
                   r => r.HasOne(typeof(Rol)).WithMany().HasForeignKey("RolId"));
    }
}
