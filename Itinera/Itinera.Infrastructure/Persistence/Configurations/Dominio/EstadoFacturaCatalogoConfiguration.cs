using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class EstadoFacturaCatalogoConfiguration : IEntityTypeConfiguration<EstadoFacturaCatalogo>
{
    public void Configure(EntityTypeBuilder<EstadoFacturaCatalogo> builder)
    {
        builder.ToTable("EstadosFactura");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasData(
            new { Id = (int)EstadoFactura.Pendiente, Descripcion = "Pendiente" },
            new { Id = (int)EstadoFactura.ParcialmentePagada, Descripcion = "Parcialmente Pagada" },
            new { Id = (int)EstadoFactura.Pagada, Descripcion = "Pagada" },
            new { Id = (int)EstadoFactura.Anulada, Descripcion = "Anulada" });
    }
}
