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

        builder.Property(x => x.FechaInicio)
               .IsRequired();

        builder.Property(x => x.FechaFin)
               .IsRequired();

        builder.Property(x => x.CantidadViajeros)
               .IsRequired();

        builder.Property(x => x.Preferencias)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(x => x.IntensidadViajeId)
               .IsRequired();

        builder.Property(x => x.EmpresaId)
               .IsRequired();

        builder.HasIndex(x => x.EmpresaId);

        builder.Ignore(x => x.Estado);

        builder.Ignore(x => x.CostoItinerarios);

        builder.Property(x => x.EstadoPropuestaId)
               .IsRequired();

        builder.HasOne(x => x.EstadoPropuestaCatalogo)
               .WithMany()
               .HasForeignKey(x => x.EstadoPropuestaId)
               .OnDelete(DeleteBehavior.Restrict);

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

        builder.HasMany(x => x.Itinerarios)
               .WithOne(x => x.Propuesta)
               .HasForeignKey(x => x.PropuestaId);

        builder.HasMany(x => x.Facturas)
               .WithOne(x => x.Propuesta)
               .HasForeignKey(x => x.PropuestaId);
    }
}
