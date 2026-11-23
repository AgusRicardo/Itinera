using Itinera.Domain.Common;

namespace Itinera.Application.Propuestas.Planificacion;

public sealed class PerfilIntensidadResolver(PlanificacionItinerarioOptions opciones)
{
    public PerfilIntensidad Resolver(IntensidadViaje intensidad)
    {
        var clave = intensidad.ToString();

        if (opciones.Perfiles.TryGetValue(clave, out var perfil))
            return perfil;

        if (opciones.Perfiles.TryGetValue(nameof(IntensidadViaje.Media), out var perfilMedio))
            return perfilMedio;

        return new PerfilIntensidad
        {
            MinutosDisponibles = 420,
            MinutosTrasladoEntreActividades = 30,
            MinutosPausaDescanso = 45,
            MinActividadesPorDia = 2,
            MaxActividadesPorDia = 5,
            PorcentajeDiasLibres = 20
        };
    }
}
