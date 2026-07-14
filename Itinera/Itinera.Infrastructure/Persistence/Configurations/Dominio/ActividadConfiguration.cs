using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class ActividadConfiguration : IEntityTypeConfiguration<Actividad>
{
    public void Configure(EntityTypeBuilder<Actividad> builder)
    {
        builder.ToTable("Actividades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.Descripcion)
               .HasMaxLength(500);

        builder.Property(x => x.CostoBase)
               .HasPrecision(18, 2);

        builder.Property(x => x.DuracionEstimada);
    }
}
