using Itinera.Domain.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.Infrastructure.Persistence.Configurations.Facturacion;

public sealed class ConceptoFacturaConfiguration : IEntityTypeConfiguration<ConceptoFactura>
{
    public void Configure(EntityTypeBuilder<ConceptoFactura> builder)
    {
        builder.ToTable("ConceptosFactura");
        builder.HasKey(concepto => concepto.Id);
        builder.Property(concepto => concepto.Descripcion)
            .IsRequired()
            .HasMaxLength(300);
        builder.Property(concepto => concepto.Precio)
            .HasPrecision(18, 2);
        builder.Property(concepto => concepto.Porcentaje)
            .HasPrecision(5, 2);
        builder.HasIndex(concepto => new { concepto.EmpresaId, concepto.Descripcion })
            .IsUnique()
            .HasFilter("\"Activo\"");
        builder.HasOne<EmpresaEntidad>()
            .WithMany()
            .HasForeignKey(concepto => concepto.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
