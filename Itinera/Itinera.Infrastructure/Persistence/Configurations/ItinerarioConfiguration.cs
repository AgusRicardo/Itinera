using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations;

public class ItinerarioConfiguration : IEntityTypeConfiguration<Itinerario>
{
    public void Configure(EntityTypeBuilder<Itinerario> builder)
    {
        builder.ToTable("Itinerarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .HasMaxLength(150);

        builder.Property(x => x.FechaInicio);

        builder.Property(x => x.FechaFin);

        builder.Property(x => x.FechaRegistracion)
               .IsRequired();

        builder.Property(x => x.UsuarioRegistracionId)
               .IsRequired();

        builder.Property(x => x.FechaModificacion);

        builder.Property(x => x.UsuarioModificacionId);

        builder.HasMany(x => x.Destinos)
               .WithOne()
               .HasForeignKey("ItinerarioId")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
