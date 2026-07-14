using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class PropuestaConfiguration : IEntityTypeConfiguration<Propuesta>
{
    public void Configure(EntityTypeBuilder<Propuesta> builder)
    {
        builder.ToTable("Propuestas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FechaCreacion);

        builder.Property(x => x.Presupuesto)
               .HasPrecision(18, 2);

        builder.Property(x => x.Estado)
               .IsRequired();

        builder.HasOne(x => x.Cliente)
               .WithMany(x => x.Propuestas);

        builder.HasOne(x => x.Empleado)
               .WithMany(x => x.Propuestas);

        builder.Property(x => x.FechaRegistracion)
               .IsRequired();

        builder.Property(x => x.UsuarioRegistracionId)
               .IsRequired();

        builder.Property(x => x.FechaModificacion);

        builder.Property(x => x.UsuarioModificacionId);

        builder.HasOne(x => x.Itinerario)
               .WithOne(x => x.Propuesta)
               .HasForeignKey<Itinerario>(x => x.PropuestaId);
    }
}
