using Itinera.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.ToTable("Empleados");
        builder.HasBaseType<Itinera.Domain.Common.Persona>();

        builder.HasOne(x => x.Cargo)
               .WithMany()
               .HasForeignKey(x => x.CargoId);

        builder.HasOne(x => x.Empresa)
               .WithMany(x => x.Empleados)
               .HasForeignKey(x => x.EmpresaId);

        builder.HasMany(x => x.Propuestas)
               .WithOne(x => x.Empleado);
    }
}
