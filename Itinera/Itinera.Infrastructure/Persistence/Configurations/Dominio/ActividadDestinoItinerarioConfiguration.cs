using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class ActividadDestinoItinerarioConfiguration : IEntityTypeConfiguration<ActividadDestinoItinerario>
{
    public void Configure(EntityTypeBuilder<ActividadDestinoItinerario> builder)
    {
        builder.ToTable("ActividadesDestinoItinerario");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FechaHoraInicio);

        builder.Property(x => x.CostoFinal)
               .HasPrecision(18, 2);

        builder.Property(x => x.Observaciones)
               .HasMaxLength(500);

        builder.Property(x => x.Orden);

        builder.HasOne(x => x.Actividad)
               .WithMany()
               .HasForeignKey(x => x.ActividadId);
    }
}
