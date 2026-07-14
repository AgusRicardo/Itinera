using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder.ToTable("Ciudades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasOne(x => x.Pais)
               .WithMany(x => x.Ciudades)
               .HasForeignKey(x => x.PaisId);
    }
}
