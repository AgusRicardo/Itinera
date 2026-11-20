namespace Itinera.Security.Application.Common;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string AdministradorItinera = "AdministradorItinera";
    public const string Agente = "Agente";

    public static bool EsDePlataforma(string codigoPermiso)
        => codigoPermiso.StartsWith("plataforma.", StringComparison.Ordinal);

    public static readonly IReadOnlyList<string> PermisosDelAdministradorItinera = new List<string>
    {
        Permisos.CatalogosVer,
        Permisos.CatalogosGestionar,
        Permisos.PlataformaGestionarEmpresas,
        Permisos.PlataformaGestionarUsuarios,
        Permisos.PlataformaGestionarRoles,
        Permisos.PlataformaGestionarGrupos,
        Permisos.SeguridadGestionar
    };

    public static readonly IReadOnlyList<string> PermisosDelAgente = new List<string>
    {
        Permisos.CatalogosVer,
        Permisos.CatalogosOperativosVer,
        Permisos.CatalogosOperativosGestionar,
        Permisos.EtiquetasVer,
        Permisos.EtiquetasGestionar,
        Permisos.ClientesVer,
        Permisos.EmpleadosVer,
        Permisos.PropuestasVer,
        Permisos.PropuestasCrear,
        Permisos.PropuestasPresentar,
        Permisos.PropuestasRegistrarRespuesta,
        Permisos.ItinerariosEditar,
        Permisos.FacturacionVer,
        Permisos.FacturacionCrear,
        Permisos.FacturacionRegistrarPago
    };
}
