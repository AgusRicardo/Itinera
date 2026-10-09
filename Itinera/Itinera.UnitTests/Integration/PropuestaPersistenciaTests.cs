using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Services;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Servicios;
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
    public async Task CrearPropuesta_PersisteItinerarioGenerado()
    {
        await using var contexto = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contexto);
        var servicio = CrearServicio(contexto);

        var fechaInicio = new DateOnly(2026, 7, 1);
        var fechaFin = new DateOnly(2026, 7, 5);

        var respuesta = await servicio.CrearAsync(new CrearPropuestaRequest
        {
            ClienteId = escenario.Cliente.Id,
            EmpleadoId = escenario.Empleado.Id,
            Presupuesto = 1500m,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Preferencias = "Playa y gastronomía",
            DestinoIds = new List<int> { escenario.Destino.Id }
        });

        Assert.True(respuesta.Id > 0);
        Assert.Equal("Borrador", respuesta.Estado);
        Assert.NotNull(respuesta.Itinerario);
        Assert.NotEmpty(respuesta.Itinerario!.Destinos);
        Assert.NotEmpty(respuesta.Itinerario.Destinos[0].Actividades);

        var propuesta = await new PropuestaRepository(contexto).GetByIdAsync(respuesta.Id);

        Assert.NotNull(propuesta);
        Assert.Equal(EstadoPropuesta.Borrador, propuesta!.Estado);
        Assert.Equal(1500m, propuesta.Presupuesto);
        Assert.NotNull(propuesta.Itinerario);
        Assert.Equal(fechaInicio, propuesta.Itinerario.FechaInicio);
        Assert.Equal(fechaFin, propuesta.Itinerario.FechaFin);
        Assert.NotEmpty(propuesta.Itinerario.Destinos);
        Assert.NotEqual(Guid.Empty, propuesta.UsuarioRegistracionId);
    }

    [Fact]
    public async Task PresentarYResponder_CambiaEstados()
    {
        await using var contexto = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contexto);
        var servicio = CrearServicio(contexto);

        var creada = await servicio.CrearAsync(NuevaPropuesta(escenario));

        var presentada = await servicio.PresentarAsync(creada.Id);
        Assert.Equal("Presentada", presentada.Estado);

        var aceptada = await servicio.RegistrarRespuestaAsync(
            creada.Id,
            new RegistrarRespuestaRequest { Respuesta = RespuestaCliente.Aceptada });
        Assert.Equal("Aceptada", aceptada.Estado);
    }

    [Fact]
    public async Task ModificarItinerario_ReemplazaDestinos()
    {
        await using var contexto = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contexto);
        var servicio = CrearServicio(contexto);

        var creada = await servicio.CrearAsync(NuevaPropuesta(escenario));

        var modificada = await servicio.ModificarItinerarioAsync(
            creada.Id,
            new ModificarItinerarioRequest
            {
                FechaInicio = new DateOnly(2026, 8, 1),
                FechaFin = new DateOnly(2026, 8, 3),
                Destinos = new List<DestinoItinerarioRequest>
                {
                    new()
                    {
                        DestinoId = escenario.Destino.Id,
                        Orden = 1,
                        FechaLlegada = new DateOnly(2026, 8, 1),
                        FechaPartida = new DateOnly(2026, 8, 3),
                        Actividades = new List<ActividadDestinoItinerarioRequest>
                        {
                            new()
                            {
                                ActividadId = escenario.Actividad.Id,
                                FechaHoraInicio = new DateTime(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc),
                                CostoFinal = 120m,
                                Observaciones = "Cambio solicitado",
                                Orden = 1
                            }
                        }
                    }
                }
            });

        Assert.NotNull(modificada.Itinerario);
        Assert.Single(modificada.Itinerario!.Destinos);
        Assert.Equal(new DateOnly(2026, 8, 1), modificada.Itinerario.FechaInicio);
        Assert.Equal(120m, modificada.Itinerario.Destinos[0].Actividades[0].CostoFinal);
    }

    private static CrearPropuestaRequest NuevaPropuesta(Escenario escenario)
        => new()
        {
            ClienteId = escenario.Cliente.Id,
            EmpleadoId = escenario.Empleado.Id,
            Presupuesto = 1500m,
            FechaInicio = new DateOnly(2026, 7, 1),
            FechaFin = new DateOnly(2026, 7, 5),
            Preferencias = "Playa",
            DestinoIds = new List<int> { escenario.Destino.Id }
        };

    private static PropuestaService CrearServicio(Itinera.Infrastructure.Persistence.AppDbContext contexto)
        => new(
            new PropuestaRepository(contexto),
            new ClienteRepository(contexto),
            new EmpleadoRepository(contexto),
            new DestinoRepository(contexto),
            new ActividadRepository(contexto),
            new MockGeneradorItinerarioIA());

    private static async Task<Escenario> CrearEscenarioAsync(
        Itinera.Infrastructure.Persistence.AppDbContext contexto)
    {
        var sufijo = Guid.NewGuid().ToString("N");

        var empresa = new EmpresaEntidad($"Agencia {sufijo[..8]}", $"CUIT-{sufijo[..8]}", "555-1234");
        var cargo = new Cargo("Agente");
        contexto.Empresas.Add(empresa);
        contexto.Cargos.Add(cargo);
        await contexto.SaveChangesAsync();

        var cliente = new Cliente("Juan", "Perez", $"cliente-{sufijo}@test.com", "555-0001");
        var empleado = new Empleado("Ana", "Gomez", $"empleado-{sufijo}@test.com", "555-0002", cargo, empresa);
        contexto.Clientes.Add(cliente);
        contexto.Empleados.Add(empleado);
        await contexto.SaveChangesAsync();

        var pais = new Pais($"Pais {sufijo}");
        contexto.Paises.Add(pais);
        await contexto.SaveChangesAsync();

        var ciudad = new Ciudad($"Ciudad {sufijo}", pais);
        contexto.Ciudades.Add(ciudad);
        await contexto.SaveChangesAsync();

        var destino = new Destino(ciudad);
        contexto.Destinos.Add(destino);
        await contexto.SaveChangesAsync();

        var actividad = new Actividad("City tour", "Recorrido guiado", 100m, 3, destino);
        contexto.Actividades.Add(actividad);
        await contexto.SaveChangesAsync();

        return new Escenario(cliente, empleado, destino, actividad);
    }

    private sealed record Escenario(Cliente Cliente, Empleado Empleado, Destino Destino, Actividad Actividad);
}
