using NetArchTest.Rules;
using Xunit;

namespace Itinera.UnitTests.Architecture;

public class LayerDependencyTests
{
    [Fact]
    public void Dominio_NoDebeDependerDeOtrasCapas()
    {
        var resultado = Types.InAssembly(Arquitectura.Dominio)
            .Should()
            .NotHaveDependencyOnAny(
                "Itinera.Application",
                "Itinera.Infrastructure",
                "Itinera.Api",
                "Itinera.Security")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void Aplicacion_NoDebeDependerDeCapasExternas()
    {
        var resultado = Types.InAssembly(Arquitectura.Aplicacion)
            .Should()
            .NotHaveDependencyOnAny(
                "Itinera.Infrastructure",
                "Itinera.Api",
                "Itinera.Security")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void Seguridad_NoDebeDependerDelDominioNiDeOtrasCapas()
    {
        var resultado = Types.InAssembly(Arquitectura.Seguridad)
            .Should()
            .NotHaveDependencyOnAny(
                "Itinera.Domain",
                "Itinera.Application",
                "Itinera.Infrastructure",
                "Itinera.Api")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void Infraestructura_NoDebeDependerDeLaApi()
    {
        var resultado = Types.InAssembly(Arquitectura.Infraestructura)
            .Should()
            .NotHaveDependencyOn("Itinera.Api")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }
}
