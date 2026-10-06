using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("Personas");
        builder.UseTptMappingStrategy();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Apellido)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Telefono)
               .HasMaxLength(30);

        builder.Property(x => x.FechaRegistracion)
               .IsRequired();

        builder.Property(x => x.UsuarioRegistracionId)
               .IsRequired();

        builder.Property(x => x.FechaModificacion);

        builder.Property(x => x.UsuarioModificacionId);

        builder.Property(x => x.Activo)
               .IsRequired();

        builder.HasQueryFilter(x => x.Activo);
    }
}
