using System.IdentityModel.Tokens.Jwt;
using Itinera.Infrastructure.Persistence.Repositories.Seguridad;
using Itinera.Infrastructure.Persistence.Seed;
using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Services;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Itinera.UnitTests.Integration;

[Collection("PostgreSQL")]
[Trait("Category", "Integration")]
public class SeguridadTests
{
    private readonly PostgresFixture _fixture;

    public SeguridadTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Seeder_EsIdempotenteYCreaRolesYPermisos()
    {
        await using var contexto = _fixture.CrearContexto();
        var seeder = CrearSeeder(contexto);

        await seeder.SembrarAsync();
        await seeder.SembrarAsync();

        var codigos = await contexto.Permisos.Select(p => p.Codigo).ToListAsync();
        foreach (var definicion in Permisos.Catalogo)
            Assert.Contains(definicion.Codigo, codigos);

        var roles = await contexto.Roles.Select(rol => rol.Nombre).ToListAsync();
        Assert.Contains(Roles.Administrador, roles);
        Assert.Contains(Roles.Agente, roles);
    }

    [Fact]
    public async Task ResolverPermisos_HeredaDeGruposAnidados()
    {
        await using var contexto = _fixture.CrearContexto();
        var repositorio = new SecurityRepository(contexto);
        var sufijo = Guid.NewGuid().ToString("N");

        var permiso = new Permiso($"test.recursivo.{sufijo}", "Prueba de recursión");
        var rol = new Rol($"RolRecursivo{sufijo}", "Prueba");
        rol.AgregarPermiso(permiso);
        contexto.Permisos.Add(permiso);
        contexto.Roles.Add(rol);
        await contexto.SaveChangesAsync();

        var grupoExterno = new GrupoUsuarios($"Externo{sufijo}");
        var grupoInterno = new GrupoUsuarios($"Interno{sufijo}");
        contexto.GruposUsuarios.AddRange(grupoExterno, grupoInterno);
        await contexto.SaveChangesAsync();

        await repositorio.AgregarMiembroAsync(grupoExterno.Id, grupoInterno.Id);
        await repositorio.AsignarRolAsync(grupoExterno.Id, rol.Id);

        var usuario = new Usuario("Tester", $"rec-{sufijo}@test.com", PasswordHasher.Hash("Clave123!"));
        contexto.Usuarios.Add(usuario);
        await contexto.SaveChangesAsync();
        await repositorio.AgregarMiembroAsync(grupoInterno.Id, usuario.Id);

        var permisos = await repositorio.GetPermisosDeUsuarioAsync(usuario.Id);

        Assert.Contains(permisos, p => p.Codigo == permiso.Codigo);
    }

    [Fact]
    public async Task AgregarMiembro_RechazaCiclos()
    {
        await using var contexto = _fixture.CrearContexto();
        var repositorio = new SecurityRepository(contexto);
        var sufijo = Guid.NewGuid().ToString("N");

        var grupoA = new GrupoUsuarios($"A{sufijo}");
        var grupoB = new GrupoUsuarios($"B{sufijo}");
        contexto.GruposUsuarios.AddRange(grupoA, grupoB);
        await contexto.SaveChangesAsync();

        await repositorio.AgregarMiembroAsync(grupoA.Id, grupoB.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repositorio.AgregarMiembroAsync(grupoB.Id, grupoA.Id));
    }

    [Fact]
    public async Task Login_IncluyeClaimsDePermiso()
    {
        await using var contexto = _fixture.CrearContexto();
        var seeder = CrearSeeder(contexto);
        await seeder.SembrarAsync();

        var repositorio = new SecurityRepository(contexto);
        var email = $"login-{Guid.NewGuid():N}@test.com";
        var usuario = new Usuario("Tester", email, PasswordHasher.Hash("Clave123!"));
        contexto.Usuarios.Add(usuario);
        await contexto.SaveChangesAsync();

        var rolAdministrador = await contexto.Roles
            .FirstAsync(rol => rol.Nombre == Roles.Administrador);
        await repositorio.AsignarRolAsync(usuario.Id, rolAdministrador.Id);

        var settings = Options.Create(new JwtSettings
        {
            Key = "clave-de-prueba-con-al-menos-32-caracteres-1234",
            Issuer = "Itinera",
            Audience = "ItineraApi",
            ExpireMinutes = 60
        });
        var servicio = new AuthenticationService(repositorio, settings);

        var login = await servicio.LoginAsync(new LoginRequest(email, "Clave123!"));

        var token = new JwtSecurityTokenHandler().ReadJwtToken(login.Token);
        var permisos = token.Claims
            .Where(claim => claim.Type == Permisos.ClaimType)
            .Select(claim => claim.Value)
            .ToList();

        Assert.Contains(Permisos.SeguridadGestionar, permisos);
        Assert.Contains(Permisos.ClientesVer, permisos);
    }

    private static SecuritySeeder CrearSeeder(
        Itinera.Infrastructure.Persistence.AppDbContext contexto,
        string? adminEmail = null,
        string? adminPassword = null)
    {
        var valores = new Dictionary<string, string?>();
        if (adminEmail is not null)
            valores["Seed:Admin:Email"] = adminEmail;
        if (adminPassword is not null)
            valores["Seed:Admin:Password"] = adminPassword;

        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();

        return new SecuritySeeder(contexto, configuracion, NullLogger<SecuritySeeder>.Instance);
    }
}
