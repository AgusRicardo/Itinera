using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Auditoria;

public sealed class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("RegistrosAuditoria");
        builder.HasKey(registro => registro.Id);

        builder.Property(registro => registro.Accion)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.Entidad)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.EntidadId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(registro => registro.Detalle)
            .HasMaxLength(2000);

        builder.Property(registro => registro.Fecha)
            .IsRequired();

        builder.HasIndex(registro => new { registro.EmpresaId, registro.Fecha });
        builder.HasIndex(registro => new { registro.Entidad, registro.EntidadId });
    }
}
