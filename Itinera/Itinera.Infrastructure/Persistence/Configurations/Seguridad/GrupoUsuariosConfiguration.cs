using Itinera.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class GrupoUsuariosConfiguration : IEntityTypeConfiguration<GrupoUsuarios>
{
    public void Configure(EntityTypeBuilder<GrupoUsuarios> builder)
    {
        builder.HasMany(g => g.GrupoMiembros)
               .WithOne(gm => gm.Grupo)
               .HasForeignKey(gm => gm.GrupoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
