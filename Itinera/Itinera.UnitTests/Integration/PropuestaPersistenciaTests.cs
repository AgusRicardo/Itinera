using Itinera.Application.Common.Exceptions;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Mappers;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Application.Propuestas.Services;
using Itinera.Application.Common.Interfaces;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Servicios;
using Itinera.UnitTests.Common;
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
    public async Task CrearPropuesta_PersisteConfiguracionSinItinerarios()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        await using var contexto = _fixture.CrearContextoConEmpresa(empresaId);
        var servicio = CrearServicio(contexto, empresaId);

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
            Destinos = [NuevaEstadia(escenario.Destino.Id)],
            ActividadIds = new List<int> { escenario.Actividad.Id }
        });

        Assert.True(respuesta.Id > 0);
        Assert.Equal("Borrador", respuesta.Estado);
        Assert.Null(Assert.Single(respuesta.Destinos).Itinerario);
        Assert.Single(respuesta.Actividades);

        var propuesta = await new PropuestaRepository(contexto).GetByIdAsync(respuesta.Id);

        Assert.NotNull(propuesta);
        Assert.Equal(EstadoPropuesta.Borrador, propuesta!.Estado);
        Assert.Equal(1500m, propuesta.Presupuesto);
        Assert.Empty(propuesta.Itinerarios);
        Assert.Single(propuesta.Destinos);
        Assert.Single(propuesta.Actividades);
        Assert.Equal(fechaInicio, propuesta.FechaInicio);
        Assert.Equal(fechaFin, propuesta.FechaFin);
        Assert.NotEqual(Guid.Empty, propuesta.UsuarioRegistracionId);

        var auditoria = await contexto.RegistrosAuditoria
            .SingleAsync(registro =>
                registro.Entidad == nameof(Propuesta)
                && registro.EntidadId == respuesta.Id.ToString()
                && registro.Accion == nameof(AccionAuditoria.CrearPropuesta));

        Assert.Equal(respuesta.Id.ToString(), auditoria.EntidadId);
    }

    [Fact]
    public async Task PresentarYResponder_CambiaEstados()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        await using var contexto = _fixture.CrearContextoConEmpresa(empresaId);
        var servicio = CrearServicio(contexto, empresaId);

        var creada = await servicio.CrearAsync(NuevaPropuesta(escenario));
        await AgregarActividadAsync(servicio, creada.Id, escenario);

        var presentada = await servicio.PresentarAsync(creada.Id);
        Assert.Equal("Presentada", presentada.Estado);

        var aceptada = await servicio.RegistrarRespuestaAsync(
            creada.Id,
            new RegistrarRespuestaRequest { Respuesta = RespuestaCliente.Aceptada });
        Assert.Equal("Aceptada", aceptada.Estado);
    }

    [Fact]
    public async Task GuardarItinerario_CreaUnoPorDestinoYLoReemplaza()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        await using var contexto = _fixture.CrearContextoConEmpresa(empresaId);
        var servicio = CrearServicio(contexto, empresaId);

        var creada = await servicio.CrearAsync(NuevaPropuesta(escenario));
        var primera = await AgregarActividadAsync(servicio, creada.Id, escenario);
        var segunda = await AgregarActividadAsync(servicio, creada.Id, escenario, 15);

        await using var verificacion = _fixture.CrearContextoConEmpresa(empresaId);
        var guardada = await new PropuestaRepository(verificacion).GetByIdAsync(creada.Id);
        var itinerario = Assert.Single(guardada!.Itinerarios);

        Assert.Equal(primera.Destinos[0].Itinerario!.Id, segunda.Destinos[0].Itinerario!.Id);
        Assert.Equal(15, Assert.Single(itinerario.Actividades).FechaHoraInicio.Hour);
    }

    [Fact]
    public async Task Actualizar_ModificaConfiguracionYConservaItinerarios()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        await using var contexto = _fixture.CrearContextoConEmpresa(empresaId);
        var servicio = CrearServicio(contexto, empresaId);
        var creada = await servicio.CrearAsync(NuevaPropuesta(escenario));
        await AgregarActividadAsync(servicio, creada.Id, escenario);

        var actualizada = await servicio.ActualizarAsync(creada.Id, new ActualizarPropuestaRequest
        {
            Presupuesto = 2000,
            CantidadViajeros = 3,
            FechaInicio = new DateOnly(2026, 7, 1),
            FechaFin = new DateOnly(2026, 7, 9),
            Destinos = [NuevaEstadia(escenario.Destino.Id, 3)],
            ActividadIds = [escenario.Actividad.Id]
        });

        await using var verificacion = _fixture.CrearContextoConEmpresa(empresaId);
        var guardada = await new PropuestaRepository(verificacion).GetByIdAsync(creada.Id);

        Assert.Equal(300, guardada!.CostoItinerarios);
        Assert.Equal(2000, guardada.Presupuesto);
        Assert.Equal("Hay actividades fuera de las fechas del destino.", Assert.Single(actualizada.Destinos[0].Faltantes));
    }

    [Fact]
    public async Task CambiarDestino_ReemplazaDestinoItinerarioYPedidas()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var otro = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        await using var contexto = _fixture.CrearContextoConEmpresa(empresaId);
        var creada = await CrearServicio(contexto, empresaId).CrearAsync(NuevaPropuesta(escenario));
        await AgregarActividadAsync(CrearServicio(contexto, empresaId), creada.Id, escenario);
        var generador = CrearGenerador(contexto, empresaId);

        var alternativa = Assert.Single(await generador.GenerarCambioDestinoAsync(
            creada.Id,
            escenario.Destino.Id,
            new GenerarCambioDestinoRequest { NuevoDestinoId = otro.Destino.Id }));
        await generador.CambiarDestinoAsync(
            creada.Id,
            escenario.Destino.Id,
            new CambiarDestinoRequest { NuevoDestinoId = otro.Destino.Id, Alternativa = alternativa });

        await using var verificacion = _fixture.CrearContextoConEmpresa(empresaId);
        var guardada = await new PropuestaRepository(verificacion).GetByIdAsync(creada.Id);

        Assert.Equal(otro.Destino.Id, Assert.Single(guardada!.Destinos).DestinoId);
        Assert.Equal(otro.Destino.Id, Assert.Single(guardada.Itinerarios).DestinoId);
        Assert.Empty(guardada.Actividades);
        Assert.True(guardada.TieneItinerariosCompletos());
        Assert.True(await verificacion.Actividades.AnyAsync(item =>
            item.DestinoId == otro.Destino.Id && item.GeneradoPorIA));
    }

    [Fact]
    public async Task CatalogoDesactivado_SigueVisibleYEditableEnLaPropuestaQueLoUsa()
    {
        await using var contextoInicial = _fixture.CrearContexto();
        var escenario = await CrearEscenarioAsync(contextoInicial);
        var empresaId = escenario.Empleado.EmpresaId;
        int propuestaId;
        await using (var contexto = _fixture.CrearContextoConEmpresa(empresaId))
        {
            var servicio = CrearServicio(contexto, empresaId);
            propuestaId = (await servicio.CrearAsync(NuevaPropuesta(escenario))).Id;
            await AgregarActividadAsync(servicio, propuestaId, escenario);
        }

        escenario.Actividad.Desactivar();
        escenario.Destino.Desactivar();
        escenario.Destino.Ciudad.Desactivar();
        escenario.Destino.Ciudad.Pais.Desactivar();
        escenario.Empleado.Cargo.Desactivar();
        await contextoInicial.SaveChangesAsync();

        await using var posterior = _fixture.CrearContextoConEmpresa(empresaId);
        var servicioPosterior = CrearServicio(posterior, empresaId);
        var actualizada = await servicioPosterior.ActualizarAsync(propuestaId, new ActualizarPropuestaRequest
        {
            Presupuesto = 2000,
            FechaInicio = new DateOnly(2026, 7, 1),
            FechaFin = new DateOnly(2026, 7, 5),
            Destinos = [NuevaEstadia(escenario.Destino.Id)],
            ActividadIds = [escenario.Actividad.Id]
        });
        await AgregarActividadAsync(servicioPosterior, propuestaId, escenario, hora: 11);

        var destino = Assert.Single(actualizada.Destinos);
        Assert.Contains(escenario.Destino.Ciudad.Nombre, destino.Descripcion);
        Assert.Single(destino.Itinerario!.Actividades);
        Assert.Single(actualizada.Actividades);

        await using var otro = _fixture.CrearContextoConEmpresa(empresaId);
        await Assert.ThrowsAsync<DestinoNoEncontradoException>(
            () => CrearServicio(otro, empresaId).CrearAsync(NuevaPropuesta(escenario)));
    }

    private static EstadiaRequest NuevaEstadia(int destinoId, int dia = 1)
        => new()
        {
            DestinoId = destinoId,
            FechaLlegada = new DateOnly(2026, 7, dia),
            FechaPartida = new DateOnly(2026, 7, 5)
        };

    private static CrearPropuestaRequest NuevaPropuesta(Escenario escenario)
        => new()
        {
            ClienteId = escenario.Cliente.Id,
            EmpleadoId = escenario.Empleado.Id,
            Presupuesto = 1500m,
            FechaInicio = new DateOnly(2026, 7, 1),
            FechaFin = new DateOnly(2026, 7, 5),
            Preferencias = "Playa",
            Destinos = [NuevaEstadia(escenario.Destino.Id)],
            ActividadIds = new List<int> { escenario.Actividad.Id }
        };

    private static Task<PropuestaResponse> AgregarActividadAsync(
        PropuestaService servicio,
        int propuestaId,
        Escenario escenario,
        int hora = 9)
        => servicio.GuardarItinerarioAsync(
            propuestaId,
            escenario.Destino.Id,
            new GuardarItinerarioRequest
            {
                Actividades =
                [
                    new ActividadItinerarioRequest
                    {
                        ActividadId = escenario.Actividad.Id,
                        FechaHoraInicio = new DateTime(2026, 7, 1, hora, 0, 0, DateTimeKind.Utc)
                    }
                ]
            });

    private static PropuestaService CrearServicio(AppDbContext contexto, int empresaId)
        => new(
            new PropuestaRepository(contexto),
            new ClienteRepository(contexto, new TenantContextFake { EmpresaId = empresaId }),
            new EmpleadoRepository(contexto, new TenantContextFake { EmpresaId = empresaId }),
            new DestinoRepository(contexto),
            new EtiquetaRepository(contexto, new TenantContextFake { EmpresaId = empresaId }),
            new ArmadorItinerario(new ActividadRepository(contexto)),
            CrearMapper(),
            CrearAuditoria(contexto, empresaId));

    private static GeneradorItinerariosService CrearGenerador(AppDbContext contexto, int empresaId)
    {
        var opciones = new PlanificacionItinerarioOptions();

        return new GeneradorItinerariosService(
            new PropuestaRepository(contexto),
            new GeneradorItinerarioIAFake(),
            new PerfilIntensidadResolver(opciones),
            new ActividadRepository(contexto),
            new DestinoRepository(contexto),
            new AjustadorItinerarioGenerado(opciones),
            new ValidadorItinerarioGenerado(opciones),
            new ArmadorItinerario(new ActividadRepository(contexto)),
            CrearMapper(),
            CrearAuditoria(contexto, empresaId));
    }

    private static PropuestaMapper CrearMapper()
    {
        var opciones = new PlanificacionItinerarioOptions();

        return new PropuestaMapper(new PerfilIntensidadResolver(opciones), opciones);
    }

    private static AuditoriaService CrearAuditoria(AppDbContext contexto, int empresaId)
        => new(contexto, new UsuarioDePrueba(), new TenantContextFake { EmpresaId = empresaId });

    private sealed class UsuarioDePrueba : ICurrentUserService
    {
        public Guid? UsuarioId { get; } =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        public bool TienePermiso(string codigoPermiso) => true;
    }

    private static async Task<Escenario> CrearEscenarioAsync(
        AppDbContext contexto)
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

        contexto.EmpresaClientes.Add(new EmpresaCliente(empresa.Id, cliente.Id));
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
