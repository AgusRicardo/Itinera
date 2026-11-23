using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Propuestas;

public sealed class PropuestaActividadConfiguration : IEntityTypeConfiguration<PropuestaActividad>
{
    public void Configure(EntityTypeBuilder<PropuestaActividad> builder)
    {
        builder.ToTable("PropuestasActividades");
        builder.HasKey(item => new { item.PropuestaId, item.ActividadId });

        builder.HasOne(item => item.Propuesta)
            .WithMany(propuesta => propuesta.Actividades)
            .HasForeignKey(item => item.PropuestaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Actividad)
            .WithMany()
            .HasForeignKey(item => item.ActividadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
