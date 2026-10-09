using Xunit;

namespace Itinera.UnitTests.Architecture;

public class NamespaceConsistencyTests
{
    [Fact]
    public void Seguridad_NoDebeTenerTiposEnElNamespaceAplicacion()
    {
        var tipos = Arquitectura.Seguridad.GetTypes()
            .Where(tipo => tipo.Namespace is not null
                && tipo.Namespace.StartsWith("Itinera.Security.Aplicacion", StringComparison.Ordinal))
            .Select(tipo => tipo.FullName)
            .ToList();

        Assert.True(tipos.Count == 0,
            "Usar 'Itinera.Security.Application'. Tipos fuera de convención: " + string.Join(", ", tipos));
    }
}
