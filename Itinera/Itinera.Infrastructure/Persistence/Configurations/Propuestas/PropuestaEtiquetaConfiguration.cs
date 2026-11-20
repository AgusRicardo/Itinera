using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Propuestas;

public sealed class PropuestaEtiquetaConfiguration : IEntityTypeConfiguration<PropuestaEtiqueta>
{
    public void Configure(EntityTypeBuilder<PropuestaEtiqueta> builder)
    {
        builder.ToTable("PropuestasEtiquetas");
        builder.HasKey(item => new { item.PropuestaId, item.EtiquetaId });

        builder.HasOne(item => item.Propuesta)
            .WithMany(propuesta => propuesta.Etiquetas)
            .HasForeignKey(item => item.PropuestaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Etiqueta)
            .WithMany()
            .HasForeignKey(item => item.EtiquetaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
