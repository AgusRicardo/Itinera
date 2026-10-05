using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Facturacion;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Importe)
               .HasPrecision(18, 2);
        builder.Property(x => x.MedioPago)
               .IsRequired()
               .HasMaxLength(100);
    }
}
