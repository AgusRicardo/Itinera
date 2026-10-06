using Itinera.Domain.Empresa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> builder)
    {
        builder.ToTable("Cargos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Descripcion)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Activo)
               .IsRequired();

        builder.HasQueryFilter(c => c.Activo);
    }
}
