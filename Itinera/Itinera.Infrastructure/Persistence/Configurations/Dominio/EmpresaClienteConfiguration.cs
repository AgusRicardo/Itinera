using Itinera.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itinera.Infrastructure.Persistence.Configurations.Dominio;

public class EmpresaClienteConfiguration : IEntityTypeConfiguration<EmpresaCliente>
{
    public void Configure(EntityTypeBuilder<EmpresaCliente> builder)
    {
        builder.ToTable("EmpresaClientes");

        builder.HasKey(x => new { x.EmpresaId, x.ClienteId });

        builder.HasOne(x => x.Empresa)
               .WithMany(x => x.Clientes)
               .HasForeignKey(x => x.EmpresaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Cliente)
               .WithMany(x => x.Empresas)
               .HasForeignKey(x => x.ClienteId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
