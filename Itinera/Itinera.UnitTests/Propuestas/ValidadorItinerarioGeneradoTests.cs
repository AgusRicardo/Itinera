using Itinera.Application.Propuestas.Models;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Common;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class ValidadorItinerarioGeneradoTests
{
    [Fact]
    public void Rechaza_una_jornada_sobrecargada()
    {
        var solicitud = CrearSolicitud();
        var itinerario = new ItinerarioGenerado(
            "Roma intensa",
            string.Empty,
            100,
            new[]
            {
                new DestinoPlanificado(
                    1,
                    solicitud.FechaInicio,
                    solicitud.FechaInicio,
                    1,
                    new[]
                    {
                        CrearActividad(300, 9),
                        CrearActividad(300, 14)
                    })
            });

        var validador = new ValidadorItinerarioGenerado(new PlanificacionItinerarioOptions());

        Assert.False(validador.EsValido(itinerario, solicitud));
    }

    [Fact]
    public void Acepta_una_jornada_dentro_del_perfil()
    {
        var solicitud = CrearSolicitud();
        var itinerario = new ItinerarioGenerado(
            "Roma moderada",
            string.Empty,
            100,
            new[]
            {
                new DestinoPlanificado(
                    1,
                    solicitud.FechaInicio,
                    solicitud.FechaInicio,
                    1,
                    new[] { CrearActividad(120, 9) })
            });

        var validador = new ValidadorItinerarioGenerado(new PlanificacionItinerarioOptions());

        Assert.True(validador.EsValido(itinerario, solicitud));
    }

    private static SolicitudItinerario CrearSolicitud()
        => new(
            new DateOnly(2027, 4, 10),
            new DateOnly(2027, 4, 10),
            1000,
            2,
            string.Empty,
            Array.Empty<string>(),
            IntensidadViaje.Media,
            new PerfilIntensidad
            {
                MinutosDisponibles = 420,
                MinutosTrasladoEntreActividades = 30,
                MinutosPausaDescanso = 45,
                MaxActividadesPorDia = 5
            },
            new[]
            {
                new DestinoSolicitado(
                    1,
                    "Roma",
                    "Italia",
                    new[]
                    {
                        new ActividadSolicitada(1, "Actividad", string.Empty, 50, 120)
                    })
            });

    private static ActividadPlanificada CrearActividad(int duracion, int hora)
        => new(
            1,
            "Actividad",
            string.Empty,
            50,
            duracion,
            new DateTime(2027, 4, 10, hora, 0, 0, DateTimeKind.Utc),
            100,
            string.Empty,
            1);
}
