using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Services;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Integration;

[Collection("PostgreSQL")]
[Trait("Category", "Integration")]
public class PropuestaPersistenciaTests
{
    private readonly PostgresFixture _fixture;

    public PropuestaPersistenciaTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Migracion_GeneraSeedDeEstados()
    {
        await using var contexto = _fixture.CrearContexto();

        var estadosPropuesta = await contexto.EstadosPropuesta
            .OrderBy(estado => estado.Id)
            .Select(estado => estado.Descripcion)
            .ToListAsync();

        Assert.Equal(
            new[] { "Borrador", "Presentada", "Aceptada", "Rechazada", "Eliminada" },
            estadosPropuesta);

        var estadosFactura = await contexto.EstadosFactura
            .OrderBy(estado => estado.Id)
            .Select(estado => estado.Descripcion)
            .ToListAsync();

        Assert.Equal(
            new[] { "Pendiente", "Parcialmente Pagada", "Pagada", "Anulada" },
            estadosFactura);
    }

    [Fact]
    public async Task CrearPropuesta_PersisteYRecupera()
    {
        await using var contexto = _fixture.CrearContexto();

        var empresa = new EmpresaEntidad("Agencia Test", "20-12345678-9", "555-1234");
        var cargo = new Cargo("Agente");
        contexto.Empresas.Add(empresa);
        contexto.Cargos.Add(cargo);
        await contexto.SaveChangesAsync();

        var cliente = new Cliente("Juan", "Perez", "juan.perez@test.com", "555-0001");
        var empleado = new Empleado("Ana", "Gomez", "ana.gomez@test.com", "555-0002", cargo, empresa);
        contexto.Clientes.Add(cliente);
        contexto.Empleados.Add(empleado);
        await contexto.SaveChangesAsync();

        var servicio = new PropuestaService(
            new PropuestaRepository(contexto),
            new ClienteRepository(contexto),
            new EmpleadoRepository(contexto));

        var respuesta = await servicio.CrearAsync(new CrearPropuestaRequest
        {
            ClienteId = cliente.Id,
            EmpleadoId = empleado.Id,
            Presupuesto = 1500m
        });

        Assert.True(respuesta.Id > 0);

        var repositorio = new PropuestaRepository(contexto);
        var propuesta = await repositorio.GetByIdAsync(respuesta.Id);

        Assert.NotNull(propuesta);
        Assert.Equal(EstadoPropuesta.Borrador, propuesta!.Estado);
        Assert.Equal(1500m, propuesta.Presupuesto);
        Assert.NotNull(propuesta.Itinerario);
        Assert.NotEqual(Guid.Empty, propuesta.UsuarioRegistracionId);
        Assert.True(propuesta.FechaRegistracion > DateTime.UtcNow.AddMinutes(-5));
    }
}
