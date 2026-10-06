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
}
