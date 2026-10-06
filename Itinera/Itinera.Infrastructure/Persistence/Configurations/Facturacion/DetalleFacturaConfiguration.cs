using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Facturacion;

public class DetalleFacturaConfiguration : IEntityTypeConfiguration<DetalleFactura>
{
    public void Configure(EntityTypeBuilder<DetalleFactura> builder)
    {
        builder.ToTable("DetallesFactura");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descripcion)
               .IsRequired()
               .HasMaxLength(300);
        builder.Property(x => x.Cantidad)
               .HasPrecision(18, 2);
        builder.Property(x => x.PrecioUnitario)
               .HasPrecision(18, 2);
        builder.Property(x => x.SubTotal)
               .HasPrecision(18, 2);
    }
}
