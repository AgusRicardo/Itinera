using Itinera.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class GrupoMiembroConfiguration : IEntityTypeConfiguration<GrupoMiembro>
{
    public void Configure(EntityTypeBuilder<GrupoMiembro> builder)
    {
        builder.ToTable("GrupoMiembros");

        builder.HasKey(x => new { x.GrupoId, x.MiembroId });

        builder.HasOne(x => x.Miembro)
               .WithMany()
               .HasForeignKey(x => x.MiembroId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
