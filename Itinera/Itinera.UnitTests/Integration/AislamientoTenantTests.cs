using Itinera.Domain.Common;
using Itinera.Domain.Facturacion;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;
using Cargo = Itinera.Domain.Empresa.Cargo;

namespace Itinera.UnitTests.Integration;

[Collection("PostgreSQL")]
[Trait("Category", "Integration")]
public class AislamientoTenantTests
{
    private readonly PostgresFixture _fixture;

    public AislamientoTenantTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Clientes_de_otra_empresa_no_son_visibles()
    {
        await using var contexto = _fixture.CrearContexto();

        var empresaA = new EmpresaEntidad("Agencia A", "30-11111111-1", "555-0001");
        var empresaB = new EmpresaEntidad("Agencia B", "30-22222222-2", "555-0002");
        contexto.Empresas.AddRange(empresaA, empresaB);
        await contexto.SaveChangesAsync();

        var clienteA = new Cliente("Ana", "Alonso", "ana@a.com", "111");
        var clienteB = new Cliente("Beto", "Bianchi", "beto@b.com", "222");
        contexto.Clientes.AddRange(clienteA, clienteB);
        await contexto.SaveChangesAsync();

        contexto.EmpresaClientes.Add(new EmpresaCliente(empresaA.Id, clienteA.Id));
        contexto.EmpresaClientes.Add(new EmpresaCliente(empresaB.Id, clienteB.Id));
        await contexto.SaveChangesAsync();

        await using var contextoA = _fixture.CrearContextoConEmpresa(empresaA.Id);
        var repositorioA = new ClienteRepository(contextoA, new TenantContextFake { EmpresaId = empresaA.Id });

        var visibles = await repositorioA.GetAllAsync();
        var noEncontrado = await repositorioA.GetByIdAsync(clienteB.Id);

        Assert.Single(visibles);
        Assert.Equal(clienteA.Id, visibles.Single().Id);
        Assert.Null(noEncontrado);
    }

    [Fact]
    public async Task Propuestas_de_otra_empresa_no_son_visibles()
    {
        await using var contexto = _fixture.CrearContexto();

        var empresaA = new EmpresaEntidad("Agencia A", "30-11111111-1", "555-0001");
        var empresaB = new EmpresaEntidad("Agencia B", "30-22222222-2", "555-0002");
        var cargo = new Cargo("Agente");
        contexto.Empresas.AddRange(empresaA, empresaB);
        contexto.Cargos.Add(cargo);
        await contexto.SaveChangesAsync();

        var empleadoA = new Empleado("Juan", "Alonso", "juan@a.com", "111", cargo, empresaA);
        var empleadoB = new Empleado("Pedro", "Bianchi", "pedro@b.com", "222", cargo, empresaB);
        var clienteA = new Cliente("Ana", "Alonso", "ana@a.com", "111");
        var clienteB = new Cliente("Beto", "Bianchi", "beto@b.com", "222");
        contexto.Empleados.AddRange(empleadoA, empleadoB);
        contexto.Clientes.AddRange(clienteA, clienteB);
        await contexto.SaveChangesAsync();

        var propuestaA = new Propuesta(clienteA, empleadoA, 1000);
        var propuestaB = new Propuesta(clienteB, empleadoB, 2000);
        contexto.Propuestas.AddRange(propuestaA, propuestaB);
        await contexto.SaveChangesAsync();

        await using var contextoA = _fixture.CrearContextoConEmpresa(empresaA.Id);

        var visibles = await contextoA.Propuestas.ToListAsync();
        var noEncontrado = await contextoA.Propuestas
            .FirstOrDefaultAsync(p => p.Id == propuestaB.Id);

        Assert.Single(visibles);
        Assert.Equal(propuestaA.Id, visibles.Single().Id);
        Assert.Null(noEncontrado);
    }

    [Fact]
    public async Task Propuesta_eliminada_no_es_visible_para_su_empresa()
    {
        await using var contexto = _fixture.CrearContexto();

        var empresa = new EmpresaEntidad("Agencia", "30-33333333-3", "555-0003");
        var cargo = new Cargo("Agente");
        contexto.Empresas.Add(empresa);
        contexto.Cargos.Add(cargo);
        await contexto.SaveChangesAsync();

        var empleado = new Empleado("Juan", "Alonso", "juan@eliminada.com", "111", cargo, empresa);
        var cliente = new Cliente("Ana", "Alonso", "ana@eliminada.com", "111");
        contexto.Empleados.Add(empleado);
        contexto.Clientes.Add(cliente);
        await contexto.SaveChangesAsync();

        var propuesta = new Propuesta(cliente, empleado, 1000);
        contexto.Propuestas.Add(propuesta);
        await contexto.SaveChangesAsync();

        propuesta.Eliminar();
        await contexto.SaveChangesAsync();

        await using var contextoEmpresa = _fixture.CrearContextoConEmpresa(empresa.Id);
        var visible = await contextoEmpresa.Propuestas.ToListAsync();

        Assert.Empty(visible);
    }

    [Fact]
    public async Task Factura_de_propuesta_eliminada_sigue_visible_para_su_empresa()
    {
        await using var contexto = _fixture.CrearContexto();

        var empresa = new EmpresaEntidad("Agencia", "30-44444444-4", "555-0004");
        var empleado = new Empleado(
            "Juan", "Alonso", "juan@factura.com", "111", new Cargo("Agente"), empresa);
        contexto.Empleados.Add(empleado);
        await contexto.SaveChangesAsync();

        var fecha = new DateOnly(2027, 4, 10);
        var propuesta = new Propuesta(
            new Cliente("Ana", "Alonso", "ana@factura.com", "111"),
            empleado,
            1000,
            fechaInicio: fecha,
            fechaFin: fecha);
        var destino = new Destino(new Ciudad("Roma", new Pais("Italia")));
        var actividad = new Actividad("Coliseo", string.Empty, 50, 60, destino);
        contexto.Actividades.Add(actividad);
        await contexto.SaveChangesAsync();
        propuesta.DefinirDestinos([new Estadia(destino, fecha, fecha)]);
        propuesta.GuardarItinerario(
            destino.Id,
            string.Empty,
            [new ActividadItinerario(
                actividad,
                fecha.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
                1,
                string.Empty,
                1)]);
        propuesta.Presentar();
        propuesta.Aceptar();

        var factura = new Factura("F-ELIM-1", DateTime.UtcNow.AddDays(30), propuesta);
        contexto.Facturas.Add(factura);
        await contexto.SaveChangesAsync();
        propuesta.Eliminar();
        await contexto.SaveChangesAsync();

        await using var contextoEmpresa = _fixture.CrearContextoConEmpresa(empresa.Id);
        var visible = Assert.Single(await new FacturaRepository(contextoEmpresa).GetAllAsync());

        Assert.Equal(factura.Id, visible.Id);
        Assert.NotNull(visible.Propuesta);
    }
}
