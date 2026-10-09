namespace Itinera.Security.Application.Common;

public sealed record PermisoDefinicion(string Codigo, string Descripcion);

public static class Permisos
{
    public const string CatalogosVer = "catalogos.ver";
    public const string CatalogosGestionar = "catalogos.gestionar";

    public const string ClientesVer = "clientes.ver";
    public const string ClientesGestionar = "clientes.gestionar";

    public const string EmpresasGestionar = "empresas.gestionar";

    public const string EmpleadosVer = "empleados.ver";
    public const string EmpleadosGestionar = "empleados.gestionar";

    public const string PropuestasVer = "propuestas.ver";
    public const string PropuestasCrear = "propuestas.crear";
    public const string PropuestasPresentar = "propuestas.presentar";
    public const string PropuestasRegistrarRespuesta = "propuestas.registrar-respuesta";
    public const string PropuestasEliminar = "propuestas.eliminar";

    public const string ItinerariosEditar = "itinerarios.editar";

    public const string FacturacionVer = "facturacion.ver";
    public const string FacturacionCrear = "facturacion.crear";
    public const string FacturacionRegistrarPago = "facturacion.registrar-pago";
    public const string FacturacionAnular = "facturacion.anular";

    public const string SeguridadGestionar = "seguridad.gestionar";

    public const string ClaimType = "permiso";

    public static readonly IReadOnlyList<PermisoDefinicion> Catalogo = new List<PermisoDefinicion>
    {
        new(CatalogosVer, "Ver catálogos (países, ciudades, destinos, actividades, cargos)"),
        new(CatalogosGestionar, "Gestionar catálogos"),
        new(ClientesVer, "Ver clientes"),
        new(ClientesGestionar, "Gestionar clientes"),
        new(EmpresasGestionar, "Gestionar empresas"),
        new(EmpleadosVer, "Ver empleados"),
        new(EmpleadosGestionar, "Gestionar empleados"),
        new(PropuestasVer, "Ver propuestas"),
        new(PropuestasCrear, "Crear propuestas"),
        new(PropuestasPresentar, "Presentar propuestas"),
        new(PropuestasRegistrarRespuesta, "Registrar respuesta del cliente"),
        new(PropuestasEliminar, "Eliminar propuestas"),
        new(ItinerariosEditar, "Editar itinerarios"),
        new(FacturacionVer, "Ver facturación"),
        new(FacturacionCrear, "Generar facturas"),
        new(FacturacionRegistrarPago, "Registrar pagos"),
        new(FacturacionAnular, "Anular facturas"),
        new(SeguridadGestionar, "Gestionar seguridad (usuarios, roles y grupos)")
    };
}
