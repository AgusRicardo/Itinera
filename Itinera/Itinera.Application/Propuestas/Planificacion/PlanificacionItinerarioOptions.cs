namespace Itinera.Application.Propuestas.Planificacion;

public sealed class PlanificacionItinerarioOptions
{
    public const string SectionName = "PlanificacionItinerario";

    public int MinutosTrasladoPorDestino { get; set; } = 90;
    public string HoraApertura { get; set; } = "09:00";
    public string HoraCierre { get; set; } = "20:00";
    public Dictionary<string, PerfilIntensidad> Perfiles { get; set; } = new();
}

public sealed class PerfilIntensidad
{
    public int MinutosDisponibles { get; set; }
    public int MinutosTrasladoEntreActividades { get; set; }
    public int MinutosPausaDescanso { get; set; }
    public int MinActividadesPorDia { get; set; } = 1;
    public int MaxActividadesPorDia { get; set; }
    public int PorcentajeDiasLibres { get; set; }

    public int MaxDiasLibres(int dias) => dias * PorcentajeDiasLibres / 100;

    public int MinutosOcupados(IReadOnlyCollection<int> duraciones, int minutosLlegada)
        => duraciones.Sum()
            + duraciones.Count * MinutosTrasladoEntreActividades
            + (duraciones.Count >= 2 ? MinutosPausaDescanso : 0)
            + minutosLlegada;
}
