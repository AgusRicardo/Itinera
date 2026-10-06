using Itinera.Domain.Propuestas;
using Xunit;

namespace Itinera.UnitTests.Propuestas;

public class DestinoTests
{
    [Fact]
    public void Crear_con_ciudad_valida_asigna_ciudad()
    {
        var ciudad = new Ciudad("Buenos Aires", new Pais("Argentina"));

        var destino = new Destino(ciudad);

        Assert.Same(ciudad, destino.Ciudad);
        Assert.Equal(ciudad.Id, destino.CiudadId);
        Assert.True(destino.Activo);
    }

    [Fact]
    public void Crear_con_ciudad_nula_lanza_excepcion()
    {
        Assert.Throws<ArgumentNullException>(() => new Destino(null!));
    }

    [Fact]
    public void ActualizarDatos_cambia_la_ciudad()
    {
        var ciudad = new Ciudad("Buenos Aires", new Pais("Argentina"));
        var otraCiudad = new Ciudad("Cordoba", new Pais("Argentina"));
        var destino = new Destino(ciudad);

        destino.ActualizarDatos(otraCiudad);

        Assert.Same(otraCiudad, destino.Ciudad);
        Assert.Equal(otraCiudad.Id, destino.CiudadId);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var destino = new Destino(new Ciudad("Buenos Aires", new Pais("Argentina")));

        destino.Desactivar();
        Assert.False(destino.Activo);

        destino.Activar();
        Assert.True(destino.Activo);
    }
}
