using Itinera.Domain.Common;
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
}
