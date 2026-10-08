using NetArchTest.Rules;
using Xunit;

namespace Itinera.UnitTests.Architecture;

public class NamingTests
{
    [Fact]
    public void Controllers_DebenTerminarEnControllerYResidirEnSuNamespace()
    {
        var controllers = Arquitectura.Controllers(Arquitectura.Api).ToList();

        Assert.NotEmpty(controllers);
        Assert.All(controllers, controller => Assert.EndsWith("Controller", controller.Name));
        Assert.All(controllers, controller => Assert.StartsWith("Itinera.Api.Controllers", controller.Namespace));
    }

    [Fact]
    public void Interfaces_DebenEmpezarConI()
    {
        var aplicacion = Types.InAssembly(Arquitectura.Aplicacion)
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .GetResult();

        var seguridad = Types.InAssembly(Arquitectura.Seguridad)
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .GetResult();

        Assert.True(aplicacion.IsSuccessful, Arquitectura.Describir(aplicacion));
        Assert.True(seguridad.IsSuccessful, Arquitectura.Describir(seguridad));
    }

    [Fact]
    public void ServiciosDeAplicacion_DebenTerminarEnService()
    {
        var servicios = Arquitectura.ClasesEnNamespaceTerminadoEn(Arquitectura.Aplicacion, ".Services").ToList();

        Assert.NotEmpty(servicios);
        Assert.All(servicios, servicio => Assert.EndsWith("Service", servicio.Name));
    }

    [Fact]
    public void Repositorios_DebenTerminarEnRepository()
    {
        var repositorios = Arquitectura
            .ClasesEnNamespaceQueContiene(Arquitectura.Infraestructura, "Persistence.Repositories")
            .ToList();

        Assert.NotEmpty(repositorios);
        Assert.All(repositorios, repositorio => Assert.EndsWith("Repository", repositorio.Name));
    }

    [Fact]
    public void Dtos_DebenTerminarEnRequestResponseODto()
    {
        var dtos = Arquitectura.ClasesEnNamespaceTerminadoEn(Arquitectura.Aplicacion, ".Dtos")
            .Concat(Arquitectura.ClasesEnNamespaceTerminadoEn(Arquitectura.Seguridad, ".Dtos"))
            .ToList();

        Assert.NotEmpty(dtos);
        Assert.All(dtos, dto => Assert.Matches(@"(Request|Response|Dto)$", dto.Name));
    }

    [Fact]
    public void Middlewares_DebenTerminarEnMiddleware()
    {
        var resultado = Types.InAssembly(Arquitectura.Api)
            .That().AreClasses().And().ResideInNamespace("Itinera.Api.Middleware")
            .Should().HaveNameEndingWith("Middleware")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }

    [Fact]
    public void ExcepcionesDeAplicacion_DebenTerminarEnException()
    {
        var resultado = Types.InAssembly(Arquitectura.Aplicacion)
            .That().AreClasses().And().ResideInNamespace("Itinera.Application.Common.Exceptions")
            .Should().HaveNameEndingWith("Exception")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Arquitectura.Describir(resultado));
    }
}
