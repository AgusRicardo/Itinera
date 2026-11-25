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
                || permiso.StartsWith("facturacion.", StringComparison.Ordinal)
                || permiso.StartsWith("etiquetas.", StringComparison.Ordinal)
                || permiso.StartsWith("catalogos.operativos.", StringComparison.Ordinal));
    }

    [Fact]
    public void Administrador_del_tenant_no_gestiona_seguridad_ni_plataforma()
    {
        Assert.DoesNotContain(Permisos.SeguridadGestionar, Roles.PermisosDelAdministrador);
        Assert.DoesNotContain(
            Roles.PermisosDelAdministrador,
            permiso => Roles.EsDePlataforma(permiso));
    }

    [Fact]
    public void Administrador_del_tenant_opera_como_agente_y_gestiona_clientes()
    {
        foreach (var permiso in Roles.PermisosDelAgente)
            Assert.Contains(permiso, Roles.PermisosDelAdministrador);

        Assert.Contains(Permisos.ClientesGestionar, Roles.PermisosDelAdministrador);
        Assert.Contains(Permisos.PropuestasEliminar, Roles.PermisosDelAdministrador);
        Assert.Contains(Permisos.FacturacionAnular, Roles.PermisosDelAdministrador);
    }

    [Fact]
    public void Agente_no_elimina_propuestas_ni_anula_facturas()
    {
        Assert.DoesNotContain(Permisos.PropuestasEliminar, Roles.PermisosDelAgente);
        Assert.DoesNotContain(Permisos.FacturacionAnular, Roles.PermisosDelAgente);
        Assert.DoesNotContain(Permisos.ClientesGestionar, Roles.PermisosDelAgente);
    }

    [Fact]
    public void Los_permisos_de_plataforma_se_identifican_por_prefijo()
    {
        Assert.True(Roles.EsDePlataforma(Permisos.PlataformaGestionarEmpresas));
        Assert.True(Roles.EsDePlataforma(Permisos.PlataformaGestionarUsuarios));
        Assert.False(Roles.EsDePlataforma(Permisos.ClientesGestionar));
    }

    [Fact]
    public void Agente_solo_lee_conceptos_y_el_administrador_los_gestiona()
    {
        Assert.Contains(Permisos.ConceptosVer, Roles.PermisosDelAgente);
        Assert.DoesNotContain(Permisos.ConceptosGestionar, Roles.PermisosDelAgente);
        Assert.Contains(Permisos.ConceptosGestionar, Roles.PermisosDelAdministrador);
    }
}
