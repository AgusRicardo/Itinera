using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class ActividadTests
{
    private static Destino CrearDestino()
        => new(new Ciudad("Buenos Aires", new Pais("Argentina")));

    [Fact]
    public void Crear_con_datos_validos_asigna_datos()
    {
        var destino = CrearDestino();

        var actividad = new Actividad("Tour", "Recorrido guiado", 1500m, 120, destino);

        Assert.Equal("Tour", actividad.Nombre);
        Assert.Equal("Recorrido guiado", actividad.Descripcion);
        Assert.Equal(1500m, actividad.CostoBase);
        Assert.Equal(120, actividad.DuracionEstimada);
        Assert.Same(destino, actividad.Destino);
        Assert.Equal(destino.Id, actividad.DestinoId);
        Assert.True(actividad.Activo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_nombre_invalido_lanza_excepcion(string? nombre)
    {
        Assert.Throws<ArgumentException>(() => new Actividad(nombre!, "desc", 100m, 60, CrearDestino()));
    }

    [Fact]
    public void Crear_con_costo_negativo_lanza_excepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Actividad("Tour", "desc", -1m, 60, CrearDestino()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Crear_con_duracion_invalida_lanza_excepcion(int duracion)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Actividad("Tour", "desc", 100m, duracion, CrearDestino()));
    }

    [Fact]
    public void Crear_con_destino_nulo_lanza_excepcion()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Actividad("Tour", "desc", 100m, 60, null!));
    }

    [Fact]
    public void ActualizarInformacion_cambia_los_datos()
    {
        var destino = CrearDestino();
        var otroDestino = CrearDestino();
        var actividad = new Actividad("Tour", "desc", 100m, 60, destino);

        actividad.ActualizarInformacion("City tour", "nueva desc", 250m, 90, otroDestino);

        Assert.Equal("City tour", actividad.Nombre);
        Assert.Equal("nueva desc", actividad.Descripcion);
        Assert.Equal(250m, actividad.CostoBase);
        Assert.Equal(90, actividad.DuracionEstimada);
        Assert.Same(otroDestino, actividad.Destino);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var actividad = new Actividad("Tour", "desc", 100m, 60, CrearDestino());

        actividad.Desactivar();
        Assert.False(actividad.Activo);

        actividad.Activar();
        Assert.True(actividad.Activo);
    }
}
