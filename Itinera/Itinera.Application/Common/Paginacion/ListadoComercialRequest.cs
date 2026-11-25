namespace Itinera.Application.Common.Paginacion;

public class ListadoComercialRequest : ListadoRequest
{
    public List<int> EstadoIds { get; set; } = [];
    public int? ClienteId { get; set; }
    public DateTimeOffset? Desde { get; set; }

    /// <summary>Límite excluido: el comienzo del día siguiente al último que se quiere ver.</summary>
    public DateTimeOffset? Hasta { get; set; }
}
