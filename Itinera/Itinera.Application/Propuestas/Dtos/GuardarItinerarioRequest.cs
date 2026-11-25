using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Itinera.Application.Propuestas.Dtos;

public sealed class GuardarItinerarioRequest
{
    [MinLength(1)]
    public List<ActividadItinerarioRequest> Actividades { get; set; } = new();
}

public sealed class ActividadItinerarioRequest
{
    public int? ActividadId { get; set; }
    public DateTime FechaHoraInicio { get; set; }

    [MaxLength(500)]
    public string Observaciones { get; set; } = string.Empty;

    [JsonIgnore]
    public bool RecomendadaPorIA { get; init; }
}
