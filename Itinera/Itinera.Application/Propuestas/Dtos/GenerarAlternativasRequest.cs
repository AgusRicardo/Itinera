using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Propuestas.Dtos;

public class GenerarAlternativasRequest
{
    [MaxLength(2000)]
    public string? Preferencias { get; set; }
}

public sealed class GenerarCambioDestinoRequest : GenerarAlternativasRequest
{
    [Range(1, int.MaxValue)]
    public int NuevoDestinoId { get; set; }
}

public sealed class CambiarDestinoRequest
{
    [Range(1, int.MaxValue)]
    public int NuevoDestinoId { get; set; }

    public AlternativaItinerarioResponse Alternativa { get; set; } = new();
}
