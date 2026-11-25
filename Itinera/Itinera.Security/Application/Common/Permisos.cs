namespace Itinera.Security.Application.Common;

public sealed record PermisoDefinicion(string Codigo, string Descripcion);

public static class Permisos
{
    public const string CatalogosVer = "catalogos.ver";
    public const string CatalogosGestionar = "catalogos.gestionar";

    // TODO: decisión pendiente sobre quién escribe los catálogos operativos (ver Roles.PermisosDelAgente).
    public const string CatalogosOperativosVer = "catalogos.operativos.ver";
    public const string CatalogosOperativosGestionar = "catalogos.operativos.gestionar";
    public const string EtiquetasVer = "etiquetas.ver";
    public const string EtiquetasGestionar = "etiquetas.gestionar";

    public const string ClientesVer = "clientes.ver";
    public const string ClientesGestionar = "clientes.gestionar";

    public const string EmpleadosVer = "empleados.ver";
    public const string EmpleadosGestionar = "empleados.gestionar";

    public const string PropuestasVer = "propuestas.ver";
    public const string PropuestasCrear = "propuestas.crear";
    public const string PropuestasEditar = "propuestas.editar";
    public const string PropuestasPresentar = "propuestas.presentar";
    public const string PropuestasRegistrarRespuesta = "propuestas.registrar-respuesta";
    public const string PropuestasEliminar = "propuestas.eliminar";

    public const string ItinerariosEditar = "itinerarios.editar";
    public const string ItinerariosGenerarIa = "itinerarios.generar-ia";

    public const string FacturacionVer = "facturacion.ver";
    public const string FacturacionCrear = "facturacion.crear";
    public const string FacturacionRegistrarPago = "facturacion.registrar-pago";
    public const string FacturacionAnular = "facturacion.anular";

    public const string ConceptosVer = "conceptos.ver";
    public const string ConceptosGestionar = "conceptos.gestionar";

    public const string SeguridadGestionar = "seguridad.gestionar";

    public const string PlataformaGestionarEmpresas = "plataforma.empresas.gestionar";
    public const string PlataformaGestionarUsuarios = "plataforma.usuarios.gestionar";
    public const string PlataformaGestionarRoles = "plataforma.roles.gestionar";
    public const string PlataformaGestionarGrupos = "plataforma.grupos.gestionar";

    public const string ClaimType = "permiso";

    public static readonly IReadOnlyList<PermisoDefinicion> Catalogo = new List<PermisoDefinicion>
    {
        new(CatalogosVer, "Ver catálogos base (cargos y estados)"),
        new(CatalogosGestionar, "Gestionar catálogos base (cargos)"),
        new(CatalogosOperativosVer, "Ver catálogos operativos"),
        new(CatalogosOperativosGestionar, "Gestionar catálogos operativos"),
        new(EtiquetasVer, "Ver etiquetas"),
        new(EtiquetasGestionar, "Gestionar etiquetas"),
        new(ClientesVer, "Ver clientes"),
        new(ClientesGestionar, "Gestionar clientes"),
        new(EmpleadosVer, "Ver empleados"),
        new(EmpleadosGestionar, "Gestionar empleados"),
        new(PropuestasVer, "Ver propuestas"),
        new(PropuestasCrear, "Crear propuestas"),
        new(PropuestasEditar, "Editar propuestas"),
        new(PropuestasPresentar, "Presentar propuestas"),
        new(PropuestasRegistrarRespuesta, "Registrar respuesta del cliente"),
        new(PropuestasEliminar, "Eliminar propuestas"),
        new(ItinerariosEditar, "Editar itinerarios"),
        new(ItinerariosGenerarIa, "Generar itinerarios con IA"),
        new(FacturacionVer, "Ver facturación"),
        new(FacturacionCrear, "Generar facturas"),
        new(FacturacionRegistrarPago, "Registrar pagos"),
        new(FacturacionAnular, "Anular facturas"),
        new(ConceptosVer, "Ver conceptos de facturación"),
        new(ConceptosGestionar, "Gestionar conceptos de facturación"),
        new(SeguridadGestionar, "Gestionar seguridad (usuarios, roles y grupos)"),
        new(PlataformaGestionarEmpresas, "Gestionar empresas de la plataforma (alta de tenants)"),
        new(PlataformaGestionarUsuarios, "Gestionar usuarios de la plataforma"),
        new(PlataformaGestionarRoles, "Gestionar roles de la plataforma"),
        new(PlataformaGestionarGrupos, "Gestionar grupos de la plataforma")
    };
}
