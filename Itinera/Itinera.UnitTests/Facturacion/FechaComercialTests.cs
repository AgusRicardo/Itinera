using Itinera.Domain.Common;
using Xunit;

namespace Itinera.UnitTests.Facturacion;

public class FechaComercialTests
{
    [Theory]
    [InlineData(2, 59, 23)]
    [InlineData(3, 0, 24)]
    public void El_dia_cambia_a_la_medianoche_argentina(int horaUtc, int minuto, int diaEsperado)
    {
        var fecha = FechaComercial.Desde(new DateTime(2026, 11, 24, horaUtc, minuto, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 11, diaEsperado), fecha);
    }
}
