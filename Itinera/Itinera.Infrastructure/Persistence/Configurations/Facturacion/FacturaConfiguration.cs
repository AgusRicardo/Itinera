using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Facturacion;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Numero)
               .IsRequired()
               .HasMaxLength(50);
        builder.HasIndex(x => x.Numero)
               .IsUnique();

        builder.Property(x => x.ImporteTotal)
               .HasPrecision(18, 2);
        builder.Property(x => x.SaldoPendiente)
               .HasPrecision(18, 2);
        builder.Ignore(x => x.Estado);

        builder.HasOne(x => x.EstadoFacturaCatalogo)
               .WithMany()
               .HasForeignKey(x => x.EstadoFacturaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Propuesta)
               .WithMany(x => x.Facturas)
               .HasForeignKey(x => x.PropuestaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Detalles)
               .WithOne(x => x.Factura)
               .HasForeignKey(x => x.FacturaId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Pagos)
               .WithOne(x => x.Factura)
               .HasForeignKey(x => x.FacturaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
