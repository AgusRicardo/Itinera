using Itinera.Application.Common.Exceptions;
using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Actividades.Interfaces;
using Itinera.Application.Common.Interfaces;
using Itinera.Application.Destinos.Interfaces;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Application.Propuestas.Interfaces;
using Itinera.Application.Propuestas.Mappers;
using Itinera.Application.Propuestas.Models;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Application.Propuestas.Services;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Propuestas;

public class GeneradorItinerariosServiceTests
{
    private static readonly DateOnly Fecha = new(2027, 4, 10);
    private readonly GeneradorFake _generador = new();
    private readonly Destino _destino;
    private readonly Actividad _coliseo;
    private readonly Actividad _vaticano;
    private readonly ActividadRepositoryFake _actividades = new();

    public GeneradorItinerariosServiceTests()
    {
        _destino = ConId(new Destino(new Ciudad("Roma", new Pais("Italia"))), 1);
        _coliseo = ConId(new Actividad("Coliseo", string.Empty, 50, 60, _destino), 1);
        _vaticano = ConId(new Actividad("Vaticano", string.Empty, 60, 60, _destino), 2);
        _destino.Actividades.AddRange([_coliseo, _vaticano]);
        _actividades.Agregar(_coliseo, _vaticano);
    }

    [Fact]
    public async Task Sin_actividades_cargadas_no_envia_el_catalogo_y_no_limita_recomendaciones()
    {
        var servicio = CrearServicio(CrearPropuesta(conColiseo: false));
        _generador.Itinerario = CrearItinerario(
            CrearPlanificada(null, "Trastevere", 9),
            CrearPlanificada(null, "Foro Romano", 13));

        var alternativa = Assert.Single(await servicio.GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest()));

        var solicitud = _generador.Solicitud!;
        Assert.Empty(Assert.Single(solicitud.Destinos).Actividades);
        Assert.Equal(2, alternativa.Actividades.Count);
    }

    [Fact]
    public async Task Las_actividades_cargadas_son_obligatorias()
    {
        var servicio = CrearServicio(CrearPropuesta(conColiseo: true));
        _generador.Itinerario = CrearItinerario(CrearPlanificada(null, "Trastevere", 9));

        Assert.Empty(await servicio.GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest()));
        Assert.Equal(1, Assert.Single(_generador.Solicitud!.Destinos[0].Actividades).ActividadId);
    }

    [Fact]
    public async Task Guarda_las_recomendaciones_nuevas_y_reutiliza_las_del_catalogo()
    {
        var servicio = CrearServicio(CrearPropuesta(conColiseo: true));
        _generador.Itinerario = CrearItinerario(
            CrearPlanificada(1, "Nombre inventado", 9),
            CrearPlanificada(99, "vaticano", 12),
            CrearPlanificada(null, "Trastevere", 15));

        var alternativa = Assert.Single(await servicio.GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest()));

        var nueva = Assert.Single(_actividades.Guardadas);
        Assert.True(nueva.GeneradoPorIA);
        Assert.Equal("Trastevere", nueva.Nombre);
        Assert.Equal(
            new int?[] { 1, 2, nueva.Id },
            alternativa.Actividades.Select(actividad => actividad.ActividadId));
        Assert.Equal("Coliseo", alternativa.Actividades[0].Nombre);
    }

    [Fact]
    public async Task Aplicar_exige_las_obligatorias_y_reemplaza_el_itinerario_del_destino()
    {
        var propuesta = CrearPropuesta(conColiseo: true);
        var servicio = CrearServicio(propuesta);

        await Assert.ThrowsAsync<ArgumentException>(
            () => servicio.AplicarAlternativaAsync(1, 1, CrearAplicacion(2)));

        Assert.Empty(propuesta.Itinerarios);

        await servicio.AplicarAlternativaAsync(1, 1, CrearAplicacion(1, 2));
        await servicio.AplicarAlternativaAsync(1, 1, CrearAplicacion(1));

        Assert.Single(Assert.Single(propuesta.Itinerarios).Actividades);
        Assert.True(propuesta.TieneItinerariosCompletos());
    }

    [Fact]
    public async Task Modificar_dia_conserva_las_obligatorias_del_dia()
    {
        var propuesta = CrearPropuesta(conColiseo: true);
        GuardarColiseo(propuesta);
        var servicio = CrearServicio(propuesta);
        _generador.Dia = new ModificacionDiaGenerada(
            Fecha, string.Empty, 0, [CrearPlanificada(null, "Trastevere", 9)]);

        Assert.Empty(await servicio.GenerarAlternativasDiaAsync(1, 1, CrearModificacion()));

        _generador.Dia = _generador.Dia with
        {
            Actividades = [CrearPlanificada(1, "Coliseo", 9), CrearPlanificada(null, "Trastevere", 13)]
        };

        var alternativa = Assert.Single(await servicio.GenerarAlternativasDiaAsync(1, 1, CrearModificacion()));
        Assert.Equal(2, alternativa.Actividades.Count);
        Assert.Single(_actividades.Guardadas);
    }

    [Fact]
    public async Task Armar_acepta_una_actividad_desactivada_solo_si_la_propuesta_ya_la_usa()
    {
        _coliseo.Desactivar();
        var armador = new ArmadorItinerario(_actividades);
        ActividadItinerarioRequest[] solicitadas =
        [
            new() { ActividadId = 1, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(9, 0)) }
        ];

        Assert.Single(await armador.ConstruirAsync(CrearPropuesta(conColiseo: true), 1, solicitadas));
        await Assert.ThrowsAsync<ActividadNoEncontradaException>(
            () => armador.ConstruirAsync(CrearPropuesta(conColiseo: false), 1, solicitadas));
    }

    [Fact]
    public async Task Armar_conserva_la_marca_de_la_IA_solo_en_lo_que_ya_recomendo()
    {
        var armador = new ArmadorItinerario(_actividades);
        var propuesta = CrearPropuesta(conColiseo: false);
        propuesta.GuardarItinerario(1, string.Empty, await armador.ConstruirAsync(propuesta, 1,
            [new() { ActividadId = 1, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(9, 0)), RecomendadaPorIA = true }]));

        var actividades = await armador.ConstruirAsync(propuesta, 1,
        [
            new() { ActividadId = 1, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(9, 0)) },
            new() { ActividadId = 2, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(12, 0)) }
        ]);

        Assert.Equal([true, false], actividades.Select(item => item.RecomendadaPorIA));
    }

    [Theory]
    [InlineData(9, 0, true)]
    [InlineData(9, 59, true)]
    [InlineData(10, 0, false)]
    public async Task Armar_rechaza_actividades_superpuestas(int hora, int minuto, bool seSuperponen)
    {
        var armador = new ArmadorItinerario(_actividades);
        ActividadItinerarioRequest[] solicitadas =
        [
            new() { ActividadId = 1, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(9, 0)) },
            new() { ActividadId = 2, FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(hora, minuto)) }
        ];
        var construir = () => armador.ConstruirAsync(CrearPropuesta(conColiseo: true), 1, solicitadas);

        if (seSuperponen)
            Assert.Contains("se superponen", (await Assert.ThrowsAsync<ArgumentException>(construir)).Message);
        else
            Assert.Equal(2, (await construir()).Count);
    }

    private GeneradorItinerariosService CrearServicio(
        Propuesta propuesta,
        PlanificacionItinerarioOptions? opciones = null)
    {
        opciones ??= new PlanificacionItinerarioOptions();

        return new GeneradorItinerariosService(
            new PropuestaRepositoryFake(propuesta),
            _generador,
            new PerfilIntensidadResolver(opciones),
            _actividades,
            new DestinoRepositoryFake(_destino),
            new AjustadorItinerarioGenerado(opciones),
            new ValidadorItinerarioGenerado(opciones),
            new ArmadorItinerario(_actividades),
            new PropuestaMapper(new PerfilIntensidadResolver(opciones), opciones),
            new AuditoriaFake());
    }

    private Propuesta CrearPropuesta(bool conColiseo)
    {
        var propuesta = new Propuesta(
            new Cliente("Ana", "Gomez", "ana@test.com", "1"),
            new Empleado("Juan", "Perez", "juan@test.com", "2", new Cargo("Agente"),
                new EmpresaEntidad("Viajes SA", "30-12345678-9", "3")),
            1000,
            fechaInicio: Fecha,
            fechaFin: Fecha);

        propuesta.DefinirDestinos([new Estadia(_destino, Fecha, Fecha)]);
        if (conColiseo)
            propuesta.DefinirActividades([_coliseo]);

        return propuesta;
    }

    [Fact]
    public async Task Intensidad_alta_descarta_alternativas_con_dias_libres_si_hay_otra_completa()
    {
        var propuesta = CrearPropuesta(conColiseo: false);
        propuesta.Actualizar(1000, 1, string.Empty, IntensidadViaje.Alta, Fecha, Fecha.AddDays(1));
        propuesta.DefinirDestinos([new Estadia(_destino, Fecha, Fecha.AddDays(1))]);
        var incompleta = CrearItinerario(CrearPlanificada(null, "Trastevere", 9));
        _generador.Itinerario = incompleta;
        _generador.Segundo = CrearItinerario(
            CrearPlanificada(null, "Foro Romano", 9),
            CrearPlanificada(null, "Villa Borghese", 9) with
            {
                FechaHoraInicio = Fecha.AddDays(1).ToDateTime(new TimeOnly(9, 0))
            });

        var opciones = new PlanificacionItinerarioOptions
        {
            Perfiles =
            {
                ["Alta"] = new PerfilIntensidad { MinutosDisponibles = 600, MaxActividadesPorDia = 7 }
            }
        };

        var alternativa = Assert.Single(await CrearServicio(propuesta, opciones)
            .GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest()));
        Assert.Equal(2, alternativa.Actividades.Count);
    }

    [Fact]
    public async Task Ajusta_dias_sobrecargados_superpuestos_y_presupuesto_sin_quitar_las_pedidas()
    {
        var propuesta = CrearPropuesta(conColiseo: true);
        _generador.Itinerario = CrearItinerario(
            CrearPlanificada(1, "Coliseo", 18),
            CrearPlanificada(null, "Uno", 9),
            CrearPlanificada(null, "Dos", 9),
            CrearPlanificada(null, "Tres", 10),
            CrearPlanificada(null, "Cuatro", 11),
            CrearPlanificada(null, "Caro", 12) with { CostoBase = 990 });

        var alternativa = Assert.Single(await CrearServicio(propuesta)
            .GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest()));

        Assert.Contains(alternativa.Actividades, actividad => actividad.ActividadId == 1);
        Assert.DoesNotContain(alternativa.Actividades, actividad => actividad.Nombre == "Caro");
        Assert.Equal(3, alternativa.Actividades.Count);
    }

    private void GuardarColiseo(Propuesta propuesta)
        => propuesta.GuardarItinerario(
            1,
            string.Empty,
            [new ActividadItinerario(
                _coliseo, Fecha.ToDateTime(new TimeOnly(9, 0)), 1, string.Empty, 1)]);

    [Fact]
    public void Presentar_exige_el_itinerario_de_todos_los_destinos()
    {
        var florencia = ConId(new Destino(new Ciudad("Florencia", new Pais("Italia"))), 2);
        var uffizi = ConId(new Actividad("Uffizi", string.Empty, 45, 60, florencia), 3);
        var propuesta = CrearPropuesta(conColiseo: true);
        propuesta.DefinirDestinos(
            [new Estadia(_destino, Fecha, Fecha), new Estadia(florencia, Fecha, Fecha)]);

        Assert.Empty(propuesta.Itinerarios);
        GuardarColiseo(propuesta);
        Assert.Throws<InvalidOperationException>(propuesta.Presentar);

        propuesta.GuardarItinerario(
            2,
            string.Empty,
            [new ActividadItinerario(uffizi, Fecha.ToDateTime(new TimeOnly(15, 0)), 1, string.Empty, 1)]);
        propuesta.Presentar();
        propuesta.Aceptar();

        Assert.Throws<InvalidOperationException>(() => GuardarColiseo(propuesta));
    }

    [Fact]
    public void Las_estadias_no_pueden_superponerse_ni_salir_del_viaje()
    {
        var florencia = ConId(new Destino(new Ciudad("Florencia", new Pais("Italia"))), 2);
        var propuesta = CrearPropuesta(conColiseo: false);
        propuesta.Actualizar(1000, 1, string.Empty, IntensidadViaje.Media, Fecha, Fecha.AddDays(6));

        Assert.Throws<ArgumentException>(() => propuesta.DefinirDestinos(
            [new Estadia(_destino, Fecha, Fecha.AddDays(4)), new Estadia(florencia, Fecha.AddDays(3), Fecha.AddDays(6))]));
        Assert.Throws<ArgumentException>(() => propuesta.DefinirDestinos(
            [new Estadia(_destino, Fecha, Fecha.AddDays(7))]));

        propuesta.DefinirDestinos(
            [new Estadia(_destino, Fecha, Fecha.AddDays(3)), new Estadia(florencia, Fecha.AddDays(3), Fecha.AddDays(6))]);
        Assert.Equal(2, propuesta.Destinos.Count);
    }

    [Fact]
    public void Quitar_un_destino_elimina_su_itinerario_y_actividades_pedidas()
    {
        var florencia = ConId(new Destino(new Ciudad("Florencia", new Pais("Italia"))), 2);
        var propuesta = CrearPropuesta(conColiseo: true);
        GuardarColiseo(propuesta);

        propuesta.DefinirDestinos([new Estadia(florencia, Fecha, Fecha)]);

        Assert.Empty(propuesta.Itinerarios);
        Assert.Empty(propuesta.Actividades);
    }

    [Fact]
    public async Task El_presupuesto_enviado_descuenta_los_otros_destinos()
    {
        var florencia = ConId(new Destino(new Ciudad("Florencia", new Pais("Italia"))), 2);
        var uffizi = ConId(new Actividad("Uffizi", string.Empty, 400, 60, florencia), 3);
        var propuesta = CrearPropuesta(conColiseo: false);
        propuesta.DefinirDestinos(
            [new Estadia(_destino, Fecha, Fecha), new Estadia(florencia, Fecha, Fecha)]);
        propuesta.GuardarItinerario(
            2,
            string.Empty,
            [new ActividadItinerario(uffizi, Fecha.ToDateTime(new TimeOnly(9, 0)), 1, string.Empty, 1)]);

        await CrearServicio(propuesta).GenerarAlternativasAsync(1, 1, new GenerarAlternativasRequest());

        Assert.Equal(600, _generador.Solicitud!.Presupuesto);
        Assert.Equal(1, Assert.Single(_generador.Solicitud.Destinos).DestinoId);
    }

    private static T ConId<T>(T entidad, int id) where T : class
    {
        typeof(T).GetProperty("Id")!.SetValue(entidad, id);
        return entidad;
    }

    private static ModificarDiaRequest CrearModificacion()
        => new() { Fecha = Fecha, Instrucciones = "Agregar algo al aire libre" };

    private static AplicarAlternativaRequest CrearAplicacion(params int[] ids)
        => new()
        {
            Alternativa = new AlternativaItinerarioResponse
            {
                Nombre = "Roma",
                Actividades = ids
                    .Select((id, indice) => new ActividadAlternativaResponse
                    {
                        ActividadId = id,
                        FechaHoraInicio = Fecha.ToDateTime(new TimeOnly(9 + indice, 0))
                    })
                    .ToList()
            }
        };

    private static ItinerarioGenerado CrearItinerario(params ActividadPlanificada[] actividades)
        => new("Roma", string.Empty, 0, [new DestinoPlanificado(1, Fecha, Fecha, 1, actividades)]);

    private static ActividadPlanificada CrearPlanificada(int? id, string nombre, int hora)
        => new(id, nombre, string.Empty, 10, 60,
            Fecha.ToDateTime(new TimeOnly(hora, 0)), 0, string.Empty, hora);

    private sealed class GeneradorFake : IGeneradorItinerarioIA
    {
        public SolicitudItinerario? Solicitud { get; private set; }
        public ItinerarioGenerado Itinerario { get; set; } = CrearItinerario();
        public ItinerarioGenerado? Segundo { get; set; }
        public ModificacionDiaGenerada? Dia { get; set; }

        public Task<IReadOnlyList<ItinerarioGenerado>> GenerarAsync(
            SolicitudItinerario solicitud,
            CancellationToken cancellationToken = default)
        {
            Solicitud = solicitud;
            return Task.FromResult<IReadOnlyList<ItinerarioGenerado>>(
                Segundo is null ? [Itinerario] : [Itinerario, Segundo]);
        }

        public Task<IReadOnlyList<ModificacionDiaGenerada>> GenerarModificacionDiaAsync(
            SolicitudModificacionDia solicitud,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ModificacionDiaGenerada>>(Dia is null ? [] : [Dia]);
    }

    private sealed class AuditoriaFake : IAuditoriaService
    {
        public Task RegistrarAsync(
            AccionAuditoria accion,
            string entidad,
            int entidadId,
            string? detalle = null,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class PropuestaRepositoryFake(Propuesta propuesta) : IPropuestaRepository
    {
        public Task AddAsync(Propuesta nueva) => Task.CompletedTask;
        public Task<Propuesta?> GetByIdAsync(int id) => Task.FromResult<Propuesta?>(propuesta);
        public Task<Pagina<Propuesta>> GetPaginaAsync(ListadoComercialRequest request) => throw new NotSupportedException();
        public Task<ResumenPropuestasResponse> GetResumenAsync() => throw new NotSupportedException();
        public Task<List<Propuesta>> GetAllAsync() => Task.FromResult(new List<Propuesta> { propuesta });
        public Task<List<Propuesta>> GetByClienteAsync(int clienteId) => GetAllAsync();
        public Task UpdateAsync(Propuesta modificada) => Task.CompletedTask;
    }

    private sealed class DestinoRepositoryFake(Destino destino) : IDestinoRepository
    {
        public Task AddAsync(Destino nuevo) => Task.CompletedTask;
        public Task<Destino?> GetByIdAsync(int id) => Task.FromResult<Destino?>(destino);
        public Task<Pagina<Destino>> GetPaginaAsync(ListadoRequest request) => throw new NotSupportedException();
        public Task<bool> ExisteAsync(int ciudadId, int? idExcluido) => Task.FromResult(false);
        public Task<List<Destino>> GetAllAsync() => Task.FromResult(new List<Destino> { destino });
        public Task<List<Destino>> GetByIdsConActividadesAsync(IEnumerable<int> ids) => GetAllAsync();
        public Task UpdateAsync(Destino modificado) => Task.CompletedTask;
        public Task DeleteAsync(Destino eliminado) => Task.CompletedTask;
    }

    private sealed class ActividadRepositoryFake : IActividadRepository
    {
        private readonly List<Actividad> _actividades = new();

        public List<Actividad> Guardadas { get; } = new();

        public void Agregar(params Actividad[] actividades) => _actividades.AddRange(actividades);

        public Task AddAsync(Actividad actividad)
        {
            ConId(actividad, 100 + Guardadas.Count);
            Guardadas.Add(actividad);
            _actividades.Add(actividad);
            return Task.CompletedTask;
        }

        public Task<Actividad?> GetByIdConInactivasAsync(int id) => GetByIdAsync(id);

        public Task<Actividad?> GetByIdAsync(int id)
            => Task.FromResult(_actividades.FirstOrDefault(actividad => actividad.Id == id));

        public Task<Pagina<Actividad>> GetPaginaAsync(ListarActividadesRequest request) => throw new NotSupportedException();
        public Task<List<Actividad>> GetAllAsync() => Task.FromResult(_actividades.ToList());
        public Task UpdateAsync(Actividad actividad) => Task.CompletedTask;
        public Task DeleteAsync(Actividad actividad) => Task.CompletedTask;
    }
}
