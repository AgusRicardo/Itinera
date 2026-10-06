using Itinera.Domain.Usuarios;
using Xunit;

namespace Itinera.UnitTests.Usuarios;

public class ClienteTests
{
    [Fact]
    public void Crear_con_datos_validos_asigna_datos()
    {
        var cliente = new Cliente("Ana", "Gomez", "ana@correo.com", "1122334455");

        Assert.Equal("Ana", cliente.Nombre);
        Assert.Equal("Gomez", cliente.Apellido);
        Assert.Equal("ana@correo.com", cliente.Email);
        Assert.Equal("1122334455", cliente.Telefono);
        Assert.True(cliente.Activo);
    }

    [Theory]
    [InlineData("", "Gomez", "ana@correo.com")]
    [InlineData("Ana", "", "ana@correo.com")]
    [InlineData("Ana", "Gomez", "")]
    public void Crear_con_datos_invalidos_lanza_excepcion(string nombre, string apellido, string email)
    {
        Assert.Throws<ArgumentException>(() => new Cliente(nombre, apellido, email, "1122334455"));
    }

    [Fact]
    public void ActualizarDatos_cambia_los_datos()
    {
        var cliente = new Cliente("Ana", "Gomez", "ana@correo.com", "1122334455");

        cliente.ActualizarDatos("Ana", "Perez", "ana.perez@correo.com", "1199887766");

        Assert.Equal("Perez", cliente.Apellido);
        Assert.Equal("ana.perez@correo.com", cliente.Email);
        Assert.Equal("1199887766", cliente.Telefono);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var cliente = new Cliente("Ana", "Gomez", "ana@correo.com", "1122334455");

        cliente.Desactivar();
        Assert.False(cliente.Activo);

        cliente.Activar();
        Assert.True(cliente.Activo);
    }
}
