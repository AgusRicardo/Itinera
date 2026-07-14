using Itinera.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Codigo)
               .IsUnique();

        builder.Property(x => x.Descripcion)
               .HasMaxLength(300);
    }
}
