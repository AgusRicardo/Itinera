using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Propuestas;

public sealed class PropuestaDestinoConfiguration : IEntityTypeConfiguration<PropuestaDestino>
{
    public void Configure(EntityTypeBuilder<PropuestaDestino> builder)
    {
        builder.ToTable("PropuestasDestinos");
        builder.Ignore(item => item.Dias);
        builder.HasKey(item => new { item.PropuestaId, item.DestinoId });

        builder.HasOne(item => item.Propuesta)
            .WithMany(propuesta => propuesta.Destinos)
            .HasForeignKey(item => item.PropuestaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Destino)
            .WithMany()
            .HasForeignKey(item => item.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
