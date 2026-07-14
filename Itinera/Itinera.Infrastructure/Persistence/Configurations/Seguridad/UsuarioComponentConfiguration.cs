using Itinera.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class UsuarioComponentConfiguration : IEntityTypeConfiguration<UsuarioComponent>
{
    public void Configure(EntityTypeBuilder<UsuarioComponent> builder)
    {
        builder.ToTable("UsuariosSeguridad");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasDiscriminator<string>("Tipo")
               .HasValue<Usuario>("Usuario")
               .HasValue<GrupoUsuarios>("Grupo");

        builder.Property<DateTime>("FechaRegistracion")
               .IsRequired()
               .HasDefaultValueSql("datetime('now')");
    }
}
