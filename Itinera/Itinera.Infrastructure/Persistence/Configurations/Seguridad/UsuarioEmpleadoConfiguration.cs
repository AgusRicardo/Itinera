using Itinera.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Seguridad;

public class UsuarioEmpleadoConfiguration : IEntityTypeConfiguration<UsuarioEmpleado>
{
    public void Configure(EntityTypeBuilder<UsuarioEmpleado> builder)
    {
        builder.ToTable("UsuariosEmpleados");

        builder.HasKey(x => x.UsuarioId);

        builder.HasIndex(x => x.EmpleadoId)
               .IsUnique();

        builder.HasOne(x => x.Usuario)
               .WithMany()
               .HasForeignKey(x => x.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Empleado)
               .WithMany()
               .HasForeignKey(x => x.EmpleadoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
