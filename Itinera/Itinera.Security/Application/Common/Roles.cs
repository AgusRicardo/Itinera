namespace Itinera.Security.Application.Common;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string AdministradorItinera = "AdministradorItinera";
    public const string Agente = "Agente";

    public static bool EsDePlataforma(string codigoPermiso)
        => codigoPermiso.StartsWith("plataforma.", StringComparison.Ordinal);

    /// <summary>
    /// Permisos del agente: opera clientes, propuestas, itinerarios, catálogos operativos y facturación.
    /// </summary>
    // TODO: los catálogos operativos (países, ciudades, destinos y actividades) son datos globales
    // sin EmpresaId, pero hoy el agente puede crearlos y editarlos.
    // Definir si la escritura queda solo para el administrador de la plataforma o se mantiene
    // en el agente y el administrador del tenant (hoy está en los dos).
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
        Permisos.PropuestasEditar,
        Permisos.PropuestasPresentar,
        Permisos.PropuestasRegistrarRespuesta,
        Permisos.ItinerariosEditar,
        Permisos.ItinerariosGenerarIa,
        Permisos.FacturacionVer,
        Permisos.FacturacionCrear,
        Permisos.FacturacionRegistrarPago,
        Permisos.ConceptosVer
    };

    /// <summary>
    /// El administrador del tenant opera como agente y además gestiona los clientes de su empresa,
    /// puede eliminar propuestas, anular facturas y administrar los conceptos de facturación.
    /// No administra usuarios, roles ni grupos: eso es exclusivo de la plataforma.
    /// </summary>
    public static readonly IReadOnlyList<string> PermisosDelAdministrador = PermisosDelAgente
        .Concat(new[]
        {
            Permisos.ClientesGestionar,
            Permisos.PropuestasEliminar,
            Permisos.FacturacionAnular,
            Permisos.ConceptosGestionar
        })
        .ToList();

    /// <summary>
    /// El administrador de la plataforma administra empresas, usuarios, roles y grupos.
    /// No opera nada propio de una empresa: ni clientes, ni propuestas, ni itinerarios, ni facturación.
    /// </summary>
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
}
