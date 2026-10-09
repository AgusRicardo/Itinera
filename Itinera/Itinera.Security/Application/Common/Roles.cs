namespace Itinera.Security.Application.Common;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Agente = "Agente";

    public static readonly IReadOnlyList<string> PermisosDelAgente = new List<string>
    {
        Permisos.CatalogosVer,
        Permisos.ClientesVer,
        Permisos.ClientesGestionar,
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
