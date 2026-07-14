using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("Paises");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(100);
    }
}
