namespace Itinera.Application.Common.Interfaces;

public interface ITenantContext
{
    int? EmpresaId { get; set; }
    bool EsAdministradorPlataforma { get; set; }
}
