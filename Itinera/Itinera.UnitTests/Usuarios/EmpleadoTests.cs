using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Usuarios;

public class EmpleadoTests
{
    private static EmpresaEntidad CrearEmpresa()
        => new("Viajes SA", "30-12345678-9", "1122334455");

    [Fact]
    public void Crear_con_datos_validos_asigna_cargo_y_empresa()
    {
        var cargo = new Cargo("Agente de viajes");
        var empresa = CrearEmpresa();

        var empleado = new Empleado("Juan", "Perez", "juan@correo.com", "1122334455", cargo, empresa);

        Assert.Equal("Juan", empleado.Nombre);
        Assert.Same(cargo, empleado.Cargo);
        Assert.Equal(cargo.Id, empleado.CargoId);
        Assert.Same(empresa, empleado.Empresa);
        Assert.Equal(empresa.Id, empleado.EmpresaId);
        Assert.True(empleado.Activo);
    }

    [Fact]
    public void Crear_con_cargo_nulo_lanza_excepcion()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Empleado("Juan", "Perez", "juan@correo.com", "1122334455", null!, CrearEmpresa()));
    }

    [Fact]
    public void Crear_con_empresa_nula_lanza_excepcion()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Empleado("Juan", "Perez", "juan@correo.com", "1122334455", new Cargo("Agente"), null!));
    }

    [Fact]
    public void ActualizarDatos_cambia_los_datos_cargo_y_empresa()
    {
        var empleado = new Empleado(
            "Juan", "Perez", "juan@correo.com", "1122334455",
            new Cargo("Agente de viajes"), CrearEmpresa());
        var nuevoCargo = new Cargo("Coordinador");
        var nuevaEmpresa = CrearEmpresa();

        empleado.ActualizarDatos("Juan", "Lopez", "juan.lopez@correo.com", "1199887766", nuevoCargo, nuevaEmpresa);

        Assert.Equal("Lopez", empleado.Apellido);
        Assert.Equal("juan.lopez@correo.com", empleado.Email);
        Assert.Same(nuevoCargo, empleado.Cargo);
        Assert.Same(nuevaEmpresa, empleado.Empresa);
    }

    [Fact]
    public void Desactivar_y_activar_cambian_el_estado()
    {
        var empleado = new Empleado(
            "Juan", "Perez", "juan@correo.com", "1122334455",
            new Cargo("Agente de viajes"), CrearEmpresa());

        empleado.Desactivar();
        Assert.False(empleado.Activo);

        empleado.Activar();
        Assert.True(empleado.Activo);
    }
}
