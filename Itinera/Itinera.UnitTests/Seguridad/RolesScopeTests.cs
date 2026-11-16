using Itinera.Security.Application.Common;
using Xunit;

namespace Itinera.UnitTests.Seguridad;

public sealed class RolesScopeTests
{
    [Fact]
    public void AdministradorItinera_no_tiene_permisos_operativos()
    {
        Assert.DoesNotContain(
            Roles.PermisosDelAdministradorItinera,
            permiso => permiso.StartsWith("clientes.", StringComparison.Ordinal)
                || permiso.StartsWith("propuestas.", StringComparison.Ordinal)
                || permiso.StartsWith("itinerarios.", StringComparison.Ordinal)
                || permiso.StartsWith("facturacion.", StringComparison.Ordinal));
    }

    [Fact]
    public void Los_permisos_de_plataforma_se_identifican_por_prefijo()
    {
        Assert.True(Roles.EsDePlataforma(Permisos.PlataformaGestionarEmpresas));
        Assert.True(Roles.EsDePlataforma(Permisos.PlataformaGestionarUsuarios));
        Assert.False(Roles.EsDePlataforma(Permisos.ClientesGestionar));
    }
}
