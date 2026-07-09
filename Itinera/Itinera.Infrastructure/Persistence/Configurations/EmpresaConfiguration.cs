using Itinera.Domain.Empresa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RazonSocial)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.CUIT)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(e => e.Telefono)
               .HasMaxLength(30);

        builder.Property(e => e.FechaRegistracion)
               .IsRequired();

        builder.Property(e => e.UsuarioRegistracionId)
               .IsRequired();

        builder.Property(e => e.FechaModificacion);

        builder.Property(e => e.UsuarioModificacionId);
    }
}
