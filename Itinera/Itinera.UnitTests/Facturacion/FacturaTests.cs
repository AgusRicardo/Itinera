using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Facturacion;

public class FacturaTests
{
    [Fact]
    public void Registrar_pagos_debe_actualizar_saldo_y_estado()
    {
        var cliente = new Cliente("Ana", "Gomez", "ana@correo.com", "111");
        var empleado = new Empleado(
            "Juan",
            "Perez",
            "juan@correo.com",
            "222",
            new Cargo("Agente de viajes"),
            new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455"));
        var propuesta = new Propuesta(cliente, empleado, 1000);
        AgregarActividadAlItinerario(propuesta);
        propuesta.Presentar();
        propuesta.Aceptar();

        var factura = new Factura("F-0001", DateTime.UtcNow.AddDays(30), propuesta);
        factura.AgregarDetalle(new DetalleFactura("Servicio de viaje", 1, 1000));

        factura.RegistrarPago(new Pago(400, DateTime.UtcNow, "Transferencia"));

        Assert.Equal(600, factura.SaldoPendiente);
        Assert.Equal(EstadoFactura.ParcialmentePagada, factura.Estado);

        factura.RegistrarPago(new Pago(600, DateTime.UtcNow, "Transferencia"));

        Assert.Equal(0, factura.SaldoPendiente);
        Assert.Equal(EstadoFactura.Pagada, factura.Estado);
    }

    [Fact]
    public void No_debe_aceptar_pagos_con_fecha_futura()
    {
        var factura = CrearFactura();

        Assert.Throws<ArgumentException>(() =>
            factura.RegistrarPago(new Pago(100, DateTime.UtcNow.AddDays(1), "Transferencia")));
    }

    [Fact]
    public void No_debe_anularse_una_factura_con_pagos()
    {
        var factura = CrearFactura();
        factura.RegistrarPago(new Pago(100, DateTime.UtcNow, "Transferencia"));

        Assert.Throws<InvalidOperationException>(() => factura.Anular());
    }

    [Fact]
    public void No_debe_crearse_con_vencimiento_anterior_a_hoy()
    {
        var propuesta = CrearPropuestaAceptada();

        Assert.Throws<ArgumentException>(() =>
            new Factura("FAC-000001", FechaComercial.Hoy().AddDays(-1), propuesta));
    }

    [Fact]
    public void Un_detalle_porcentual_se_calcula_sobre_el_costo_del_viaje()
    {
        var detalle = DetalleFactura.PorPorcentaje("Comisión", 12.5m, 5000.1m);

        Assert.Equal((625.01m, 12.5m), (detalle.SubTotal, detalle.Porcentaje));
        Assert.Throws<InvalidOperationException>(() => DetalleFactura.PorPorcentaje("Comisión", 10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => DetalleFactura.PorPorcentaje("Comisión", 101, 5000));
    }

    private static Factura CrearFactura()
    {
        var factura = new Factura(
            "FAC-000001",
            DateTime.UtcNow.AddDays(30),
            CrearPropuestaAceptada());
        factura.AgregarDetalle(new DetalleFactura("Servicio de viaje", 1, 1000));
        return factura;
    }

    private static Propuesta CrearPropuestaAceptada()
    {
        var cliente = new Cliente("Ana", "Gomez", "ana@correo.com", "111");
        var empleado = new Empleado(
            "Juan",
            "Perez",
            "juan@correo.com",
            "222",
            new Cargo("Agente de viajes"),
            new EmpresaEntidad("Viajes SA", "30-12345678-9", "1122334455"));
        var propuesta = new Propuesta(cliente, empleado, 1000);
        AgregarActividadAlItinerario(propuesta);
        propuesta.Presentar();
        propuesta.Aceptar();
        return propuesta;
    }

    private static void AgregarActividadAlItinerario(Propuesta propuesta)
    {
        var pais = new Pais("Italia");
        var ciudad = new Ciudad("Roma", pais);
        var destino = new Destino(ciudad);
        var actividad = new Actividad("Visita", "Visita", 0, 60, destino);

        propuesta.DefinirDestinos([new Estadia(destino, propuesta.FechaInicio, propuesta.FechaFin)]);
        propuesta.GuardarItinerario(
            destino.Id,
            string.Empty,
            [new ActividadItinerario(
                actividad,
                propuesta.FechaInicio.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
                1,
                string.Empty,
                1)]);
    }
}
