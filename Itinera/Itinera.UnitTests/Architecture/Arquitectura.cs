using System.Reflection;
using System.Runtime.CompilerServices;
using Itinera.Api.Controllers;
using Itinera.Application.Propuestas.Services;
using Itinera.Domain.Propuestas;
using Itinera.Infrastructure.Persistence;
using NetArchTest.Rules;

namespace Itinera.UnitTests.Architecture;

internal static class Arquitectura
{
    public static readonly Assembly Dominio = typeof(Propuesta).Assembly;
    public static readonly Assembly Aplicacion = typeof(PropuestaService).Assembly;
    public static readonly Assembly Infraestructura = typeof(AppDbContext).Assembly;
    public static readonly Assembly Seguridad = typeof(Itinera.Security.DependencyInjection).Assembly;
    public static readonly Assembly Api = typeof(ApiController).Assembly;

    public static string Describir(TestResult resultado)
        => resultado.FailingTypeNames is null || !resultado.FailingTypeNames.Any()
            ? "Regla de arquitectura incumplida."
            : "Tipos que incumplen la regla: " + string.Join(", ", resultado.FailingTypeNames);

    public static IEnumerable<Type> ClasesEnNamespaceTerminadoEn(Assembly assembly, string sufijo)
        => ClasesDe(assembly)
            .Where(tipo => tipo.Namespace!.EndsWith(sufijo, StringComparison.Ordinal));

    public static IEnumerable<Type> ClasesEnNamespaceQueContiene(Assembly assembly, string fragmento)
        => ClasesDe(assembly)
            .Where(tipo => tipo.Namespace!.Contains(fragmento, StringComparison.Ordinal));

    public static IEnumerable<Type> Controllers(Assembly assembly)
        => ClasesDe(assembly)
            .Where(tipo => tipo != typeof(ApiController)
                && typeof(ApiController).IsAssignableFrom(tipo));

    private static IEnumerable<Type> ClasesDe(Assembly assembly)
        => assembly.GetTypes()
            .Where(tipo => tipo.IsClass
                && !tipo.IsAbstract
                && !tipo.IsNested
                && tipo.Namespace is not null
                && !tipo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false));
}
