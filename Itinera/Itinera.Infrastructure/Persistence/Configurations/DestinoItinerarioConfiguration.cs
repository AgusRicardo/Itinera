using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations;

public class DestinoItinerarioConfiguration : IEntityTypeConfiguration<DestinoItinerario>
{
    public void Configure(EntityTypeBuilder<DestinoItinerario> builder)
    {
        builder.ToTable("DestinosItinerario");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Orden);

        builder.Property(x => x.FechaLlegada);

        builder.Property(x => x.FechaPartida);

        builder.HasOne(x => x.Destino)
               .WithMany()
               .HasForeignKey("DestinoId");

        builder.HasMany(x => x.ActividadDestinoItinerarios)
               .WithOne(x => x.DestinoItinerario)
               .HasForeignKey(x => x.DestinoItinerarioId);
    }
}
