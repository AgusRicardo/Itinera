using System.Globalization;
using System.Text;

namespace Itinera.Application.Common.Textos;

public static class TextoBusqueda
{
    /// <summary>Sin espacios en los extremos, en minúsculas y sin tildes.</summary>
    public static string Normalizar(string texto)
        => string.Concat(texto.Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD)
            .Where(letra => CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark));
}
