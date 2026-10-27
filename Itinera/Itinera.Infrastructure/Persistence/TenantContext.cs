using Itinera.Application.Common.Interfaces;

namespace Itinera.Infrastructure.Persistence;

public sealed class TenantContext : ITenantContext
{
    public int? EmpresaId { get; set; }
    public bool EsAdministradorPlataforma { get; set; }
}
