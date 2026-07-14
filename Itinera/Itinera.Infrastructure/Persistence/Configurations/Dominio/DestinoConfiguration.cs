using Itinera.Domain.Propuestas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class DestinoConfiguration : IEntityTypeConfiguration<Destino>
{
    public void Configure(EntityTypeBuilder<Destino> builder)
    {
        builder.ToTable("Destinos");

        builder.HasKey(c => c.Id);

        builder.HasOne(d => d.Ciudad)
               .WithMany(c => c.Destinos)
               .HasForeignKey(d => d.CiudadId);

        builder.HasMany(d => d.Actividades)
               .WithOne(a => a.Destino)
               .HasForeignKey(a => a.DestinoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
