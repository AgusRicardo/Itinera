using NetArchTest.Rules;
using Xunit;

namespace Itinera.UnitTests.Architecture;

public class PurityTests
{
    [Fact]
    public void Dominio_NoDebeDependerDeTecnologiasDeInfraestructura()
    {
        var resultado = Types.InAssembly(Arquitectura.Dominio)
            .Should()
            .NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql",
                "Microsoft.Extensions")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void Aplicacion_NoDebeDependerDePersistenciaNiDeAspNetCore()
    {
        var resultado = Types.InAssembly(Arquitectura.Aplicacion)
            .Should()
            .NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void Seguridad_NoDebeDependerDePersistenciaNiDeAspNetCore()
    {
        var resultado = Types.InAssembly(Arquitectura.Seguridad)
            .Should()
            .NotHaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }
}
