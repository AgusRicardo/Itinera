using Itinera.Application.Common.Interfaces;
using Itinera.Infrastructure.Persistence;
using Itinera.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Itinera.UnitTests.Integration;

public sealed class PostgresFixture : IAsyncLifetime
{
    private static string CadenaConexion =>
        Environment.GetEnvironmentVariable("ITINERA_TEST_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=itinera_test;Username=itinera;Password=itinera_dev";

    public async Task InitializeAsync()
    {
        await using var contexto = CrearContexto();
        await contexto.Database.EnsureDeletedAsync();
        await contexto.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public AppDbContext CrearContexto()
        => CrearContextoConEmpresa(null);

    public AppDbContext CrearContextoConEmpresa(int? empresaId)
    {
        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(CadenaConexion)
            .AddInterceptors(new AuditInterceptor(new UsuarioDePrueba()))
            .Options;

        return new AppDbContext(opciones, new TenantContextFake { EmpresaId = empresaId });
    }

    private sealed class UsuarioDePrueba : ICurrentUserService
    {
        public Guid? UsuarioId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public bool TienePermiso(string codigoPermiso) => true;
    }
}

[CollectionDefinition("PostgreSQL")]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
}
