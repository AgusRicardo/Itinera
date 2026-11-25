namespace Itinera.Domain.Common;

/// <summary>Día calendario de las agencias (Argentina, UTC-3 sin horario de verano).</summary>
public static class FechaComercial
{
    private static readonly TimeSpan DiferenciaConUtc = TimeSpan.FromHours(-3);

    public static DateTime Desde(DateTime fechaUtc) => fechaUtc.Add(DiferenciaConUtc).Date;

    public static DateTime Hoy() => Desde(DateTime.UtcNow);
}
