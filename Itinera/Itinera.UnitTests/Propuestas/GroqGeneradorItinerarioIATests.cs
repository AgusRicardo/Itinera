using System.Net;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Propuestas.Models;
using Itinera.Application.Propuestas.Planificacion;
using Itinera.Domain.Common;
using Itinera.Infrastructure.Servicios.IA;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class GroqGeneradorItinerarioIATests
{
    [Fact]
    public async Task Usa_el_modelo_alternativo_cuando_el_principal_esta_saturado()
    {
        var manejador = new ManejadorFake("principal");

        var alternativas = await CrearGenerador(manejador).GenerarAsync(CrearSolicitud());

        Assert.Empty(alternativas);
        Assert.Equal(3, manejador.Pedidos.Count(pedido => pedido.Contains("\"model\":\"principal\"")));
        Assert.Single(manejador.Pedidos, pedido => pedido.Contains("\"model\":\"respaldo\""));
    }

    [Fact]
    public async Task Informa_servicio_no_disponible_cuando_todos_estan_saturados()
        => await Assert.ThrowsAsync<ServicioIANoDisponibleException>(() =>
            CrearGenerador(new ManejadorFake("principal", "respaldo")).GenerarAsync(CrearSolicitud()));

    [Fact]
    public async Task Pide_el_ritmo_de_la_intensidad_y_lee_el_formato_compacto()
    {
        var manejador = new ManejadorFake
        {
            Json = """{"alternativas":[{"nombre":"N","descripcion":"D","dias":{"2027-04-10":[["09:30",7,"Templo",90,12.5],["mal"]],"x":[]}}]}"""
        };
        var solicitud = CrearSolicitud() with
        {
            IntensidadViaje = IntensidadViaje.Alta,
            Perfil = new PerfilIntensidad { MinActividadesPorDia = 3, MaxActividadesPorDia = 7 },
            Destinos = [new DestinoSolicitado(1, "Tokio", "Japón", [])]
        };

        var alternativa = Assert.Single(await CrearGenerador(manejador).GenerarAsync(solicitud));

        var actividad = Assert.Single(alternativa.Destinos[0].Actividades);
        Assert.Equal(7, actividad.ActividadId);
        Assert.Equal(12.5m, actividad.CostoBase);
        Assert.Equal(new DateTime(2027, 4, 10, 9, 30, 0, DateTimeKind.Utc), actividad.FechaHoraInicio);
        Assert.Contains("Intensidad ALTA", manejador.Pedidos[0]);
        Assert.Contains("entre 3 y 7 actividades", manejador.Pedidos[0]);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, """{"choices":[{"message":{"content":"no es json"}}]}""")]
    [InlineData(HttpStatusCode.BadRequest, """{"error":{"code":"json_validate_failed","message":"x"}}""")]
    public async Task Reintenta_cuando_la_respuesta_no_es_json_valido(HttpStatusCode codigo, string contenido)
    {
        var manejador = new ManejadorFake("principal") { Codigo = codigo, Contenido = contenido };

        Assert.Empty(await CrearGenerador(manejador).GenerarAsync(CrearSolicitud()));
        Assert.Equal(4, manejador.Pedidos.Count);
    }

    [Fact]
    public async Task No_reintenta_cuando_la_key_es_invalida()
    {
        var manejador = new ManejadorFake("principal") { Codigo = HttpStatusCode.Unauthorized };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CrearGenerador(manejador).GenerarAsync(CrearSolicitud()));
        Assert.Single(manejador.Pedidos);
    }

    private static GroqGeneradorItinerarioIA CrearGenerador(ManejadorFake manejador)
        => new(
            new HttpClient(manejador),
            new GeneradorIaOptions
            {
                UrlBase = "https://api.groq.com/openai/v1",
                ApiKey = "prueba",
                Modelo = "principal",
                ModelosAlternativos = ["respaldo"],
                EsfuerzoRazonamiento = "low"
            });

    private static SolicitudItinerario CrearSolicitud()
        => new(
            new DateOnly(2027, 4, 10),
            new DateOnly(2027, 4, 10),
            1000,
            1,
            string.Empty,
            [],
            IntensidadViaje.Media,
            new PerfilIntensidad(),
            []);

    private sealed class ManejadorFake(params string[] modelosConError) : HttpMessageHandler
    {
        public List<string> Pedidos { get; } = new();
        public HttpStatusCode Codigo { get; init; } = HttpStatusCode.ServiceUnavailable;
        public string Json { get; init; } = """{"alternativas":[]}""";
        public string Contenido { get; init; } = """{"error":{"message":"saturado"}}""";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var pedido = await request.Content!.ReadAsStringAsync(cancellationToken);
            Pedidos.Add(pedido);

            Assert.Equal("https://api.groq.com/openai/v1/chat/completions", request.RequestUri!.ToString());
            Assert.Equal("Bearer prueba", request.Headers.Authorization!.ToString());
            Assert.Contains("json_object", pedido);

            if (modelosConError.Any(modelo => pedido.Contains($"\"model\":\"{modelo}\"")))
            {
                var error = new HttpResponseMessage(Codigo) { Content = new StringContent(Contenido) };
                error.Headers.RetryAfter = new(TimeSpan.FromSeconds(1));
                return error;
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = System.Net.Http.Json.JsonContent.Create(new
                {
                    choices = new[] { new { message = new { content = Json } } }
                })
            };
        }
    }
}
