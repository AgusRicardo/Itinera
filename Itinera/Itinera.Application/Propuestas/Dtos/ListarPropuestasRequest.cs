using Itinera.Application.Common.Paginacion;

namespace Itinera.Application.Propuestas.Dtos;

public class ListarPropuestasRequest : PaginacionRequest
{
    public string? Texto { get; set; }
    public int? EstadoId { get; set; }
}
