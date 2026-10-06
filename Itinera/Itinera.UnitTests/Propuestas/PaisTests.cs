using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class PaisTests
{
    [Fact]
    public void Crear_con_nombre_valido_asigna_nombre_y_queda_activo()
    {
        var pais = new Pais("Argentina");

        Assert.Equal("Argentina", pais.Nombre);
        Assert.True(pais.Activo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_nombre_invalido_lanza_excepcion(string? nombre)
    {
        Assert.Throws<ArgumentException>(() => new Pais(nombre!));
    }

    [Fact]
    public void ActualizarNombre_cambia_el_nombre()
    {
        var pais = new Pais("Argentina");

        pais.ActualizarNombre("Chile");

        Assert.Equal("Chile", pais.Nombre);
    }

    [Fact]
    public void ActualizarNombre_con_valor_invalido_lanza_excepcion()
    {
        var pais = new Pais("Argentina");

        Assert.Throws<ArgumentException>(() => pais.ActualizarNombre(" "));
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var pais = new Pais("Argentina");

        pais.Desactivar();
        Assert.False(pais.Activo);

        pais.Activar();
        Assert.True(pais.Activo);
    }
}
