namespace Itinera.Application.Common.Paginacion;

public sealed record Pagina<T>(List<T> Elementos, int NumeroPagina, int TamanoPagina, int Total)
{
    public Pagina<TDestino> Convertir<TDestino>(Func<IEnumerable<T>, List<TDestino>> convertir)
        => new(convertir(Elementos), NumeroPagina, TamanoPagina, Total);
}
