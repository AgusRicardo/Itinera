using Itinera.Application.Common.Textos;

namespace Itinera.Application.Common.Paginacion;

/// <summary>Página con búsqueda y orden por columna.</summary>
public class ListadoRequest : PaginacionRequest
{
    public string? Texto { get; set; }
    public string? Orden { get; set; }
    public bool Descendente { get; set; }

    public string? TextoNormalizado
        => string.IsNullOrWhiteSpace(Texto) ? null : TextoBusqueda.Normalizar(Texto);
}
