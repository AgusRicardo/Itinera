using Itinera.Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Catalogos;

public sealed class EtiquetaConfiguration : IEntityTypeConfiguration<Etiqueta>
{
    public void Configure(EntityTypeBuilder<Etiqueta> builder)
    {
        builder.ToTable("Etiquetas");
        builder.HasKey(etiqueta => etiqueta.Id);
        builder.Property(etiqueta => etiqueta.Nombre)
            .IsRequired()
            .HasMaxLength(80);
        builder.HasIndex(etiqueta => new { etiqueta.EmpresaId, etiqueta.Nombre })
            .IsUnique();
    }
}
