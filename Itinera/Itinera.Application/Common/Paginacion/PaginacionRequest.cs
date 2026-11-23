using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Common.Paginacion;

public class PaginacionRequest
{
    public const int TamanoMaximo = 100;

    [Range(1, int.MaxValue)]
    public int Pagina { get; set; } = 1;

    [Range(1, TamanoMaximo)]
    public int TamanoPagina { get; set; } = 10;
}
