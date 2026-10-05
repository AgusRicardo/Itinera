using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class EstadoPropuestaCatalogoConfiguration : IEntityTypeConfiguration<EstadoPropuestaCatalogo>
{
    public void Configure(EntityTypeBuilder<EstadoPropuestaCatalogo> builder)
    {
        builder.ToTable("EstadosPropuestas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasData(
            new { Id = (int)EstadoPropuesta.Borrador, Descripcion = "Borrador" },
            new { Id = (int)EstadoPropuesta.Presentada, Descripcion = "Presentada" },
            new { Id = (int)EstadoPropuesta.Aceptada, Descripcion = "Aceptada" },
            new { Id = (int)EstadoPropuesta.Rechazada, Descripcion = "Rechazada" },
            new { Id = (int)EstadoPropuesta.Eliminada, Descripcion = "Eliminada" });
    }
}
