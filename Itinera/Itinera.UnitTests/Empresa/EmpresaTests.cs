using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Empresa;

public class EmpresaTests
{
    [Fact]
    public void Crear_con_datos_validos_asigna_datos()
    {
        var empresa = new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455");

        Assert.Equal("Viajes SA", empresa.RazonSocial);
        Assert.Equal("30-12345678-9", empresa.CUIT);
        Assert.Equal("1122334455", empresa.Telefono);
        Assert.True(empresa.Activo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_razon_social_invalida_lanza_excepcion(string? razonSocial)
    {
        Assert.Throws<ArgumentException>(() => new EmpresaEntidad(razonSocial!, "30-12345678-9", "1122334455"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Crear_con_cuit_invalido_lanza_excepcion(string? cuit)
    {
        Assert.Throws<ArgumentException>(() => new EmpresaEntidad("Viajes SA", cuit!, "1122334455"));
    }

    [Fact]
    public void ActualizarDatos_cambia_los_datos()
    {
        var empresa = new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455");

        empresa.ActualizarDatos("Turismo SA", "30-98765432-1", "1199887766");

        Assert.Equal("Turismo SA", empresa.RazonSocial);
        Assert.Equal("30-98765432-1", empresa.CUIT);
        Assert.Equal("1199887766", empresa.Telefono);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var empresa = new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455");

        empresa.Desactivar();
        Assert.False(empresa.Activo);

        empresa.Activar();
        Assert.True(empresa.Activo);
    }
}
