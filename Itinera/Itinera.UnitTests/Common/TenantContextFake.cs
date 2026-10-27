using Itinera.Application.Common.Interfaces;

namespace Itinera.UnitTests.Common;

public sealed class TenantContextFake : ITenantContext
{
    public int? EmpresaId { get; set; }
    public bool EsAdministradorPlataforma { get; set; }
}
