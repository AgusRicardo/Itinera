using Itinera.Domain.Empresa;
using Xunit;

namespace Itinera.UnitTests.Empresa;

public class CargoTests
{
    [Fact]
    public void Crear_con_descripcion_valida_asigna_la_descripcion()
    {
        var cargo = new Cargo("Agente de viajes");

        Assert.Equal("Agente de viajes", cargo.Descripcion);
        Assert.True(cargo.Activo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_descripcion_invalida_lanza_excepcion(string? descripcion)
    {
        Assert.Throws<ArgumentException>(() => new Cargo(descripcion!));
    }

    [Fact]
    public void ActualizarDescripcion_cambia_la_descripcion()
    {
        var cargo = new Cargo("Agente de viajes");

        cargo.ActualizarDescripcion("Coordinador");

        Assert.Equal("Coordinador", cargo.Descripcion);
    }

    [Fact]
    public void ActualizarDescripcion_con_valor_invalido_lanza_excepcion()
    {
        var cargo = new Cargo("Agente de viajes");

        Assert.Throws<ArgumentException>(() => cargo.ActualizarDescripcion(" "));
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var cargo = new Cargo("Agente de viajes");

        cargo.Desactivar();
        Assert.False(cargo.Activo);

        cargo.Activar();
        Assert.True(cargo.Activo);
    }
}
