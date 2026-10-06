using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class CiudadTests
{
    [Fact]
    public void Crear_con_nombre_y_pais_validos_asigna_datos()
    {
        var pais = new Pais("Argentina");

        var ciudad = new Ciudad("Buenos Aires", pais);

        Assert.Equal("Buenos Aires", ciudad.Nombre);
        Assert.Same(pais, ciudad.Pais);
        Assert.Equal(pais.Id, ciudad.PaisId);
        Assert.True(ciudad.Activo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_nombre_invalido_lanza_excepcion(string? nombre)
    {
        var pais = new Pais("Argentina");

        Assert.Throws<ArgumentException>(() => new Ciudad(nombre!, pais));
    }

    [Fact]
    public void Crear_con_pais_nulo_lanza_excepcion()
    {
        Assert.Throws<ArgumentNullException>(() => new Ciudad("Buenos Aires", null!));
    }

    [Fact]
    public void ActualizarDatos_cambia_nombre_y_pais()
    {
        var argentina = new Pais("Argentina");
        var chile = new Pais("Chile");
        var ciudad = new Ciudad("Buenos Aires", argentina);

        ciudad.ActualizarDatos("Santiago", chile);

        Assert.Equal("Santiago", ciudad.Nombre);
        Assert.Same(chile, ciudad.Pais);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var ciudad = new Ciudad("Buenos Aires", new Pais("Argentina"));

        ciudad.Desactivar();
        Assert.False(ciudad.Activo);

        ciudad.Activar();
        Assert.True(ciudad.Activo);
    }
}
