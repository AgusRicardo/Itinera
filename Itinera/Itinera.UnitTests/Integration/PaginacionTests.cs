using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.UnitTests.Common;
using Xunit;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.UnitTests.Integration;

[Collection("PostgreSQL")]
[Trait("Category", "Integration")]
public class PaginacionTests(PostgresFixture fixture)
{
    [Fact]
    public async Task Clientes_PaginaFiltraYNoMuestraLosDeOtraEmpresa()
    {
        var empresaId = await CrearEmpresaConClientesAsync(12);
        await CrearEmpresaConClientesAsync(3);
        await using var contexto = fixture.CrearContextoConEmpresa(empresaId);
        var repositorio = new ClienteRepository(contexto, new TenantContextFake { EmpresaId = empresaId });

        var primera = await repositorio.GetPaginaAsync(new ListarClientesRequest());
        var segunda = await repositorio.GetPaginaAsync(new ListarClientesRequest { Pagina = 2 });
        var fueraDeRango = await repositorio.GetPaginaAsync(new ListarClientesRequest { Pagina = 99 });
        var filtrada = await repositorio.GetPaginaAsync(new ListarClientesRequest { Texto = "CLIENTE 07", Activo = true });
        var resumen = await repositorio.GetResumenAsync();

        Assert.Equal(10, primera.Elementos.Count);
        Assert.Equal(12, primera.Total);
        Assert.Equal(2, segunda.Elementos.Count);
        Assert.Empty(primera.Elementos.Select(item => item.Id).Intersect(segunda.Elementos.Select(item => item.Id)));
        Assert.Equal(2, fueraDeRango.NumeroPagina);
        Assert.Equal(2, fueraDeRango.Elementos.Count);
        Assert.Equal("Cliente 07", Assert.Single(filtrada.Elementos).Nombre);
        Assert.Equal((12, 12), (resumen.Total, resumen.Activos));
    }

    [Fact]
    public async Task Propuestas_PaginaYFiltraPorEstadoYTexto()
    {
        var empresaId = await CrearEmpresaConClientesAsync(1);
        await using (var contexto = fixture.CrearContexto())
        {
            var cargo = new Cargo("Agente");
            var empresa = await contexto.Empresas.FindAsync(empresaId);
            var empleado = new Empleado("Ana", "Gomez", $"{Guid.NewGuid():N}@e.com", "1", cargo, empresa!);
            var cliente = new Cliente("Zoe", "Unica", $"{Guid.NewGuid():N}@c.com", "1");
            contexto.AddRange(empleado, cliente);
            for (var indice = 0; indice < 11; indice++)
                contexto.Propuestas.Add(new Propuesta(cliente, empleado, 1000));
            await contexto.SaveChangesAsync();
        }

        await using var lectura = fixture.CrearContextoConEmpresa(empresaId);
        var repositorio = new PropuestaRepository(lectura);

        var primera = await repositorio.GetPaginaAsync(new ListarPropuestasRequest());
        var segunda = await repositorio.GetPaginaAsync(new ListarPropuestasRequest { Pagina = 2, Texto = "zoe u" });
        var presentadas = await repositorio.GetPaginaAsync(
            new ListarPropuestasRequest { EstadoId = (int)EstadoPropuesta.Presentada });
        var resumen = await repositorio.GetResumenAsync();

        Assert.Equal((10, 11), (primera.Elementos.Count, primera.Total));
        Assert.Single(segunda.Elementos);
        Assert.Empty(presentadas.Elementos);
        Assert.Equal((11, 11, 0), (resumen.Total, resumen.Borradores, resumen.Aceptadas));
    }

    [Fact]
    public async Task Catalogos_PaginanConTotalYSinRepetidos()
    {
        await using var contexto = fixture.CrearContexto();
        var prefijo = Guid.NewGuid().ToString("N");
        for (var indice = 0; indice < 11; indice++)
        {
            var ciudad = new Ciudad($"Ciudad {indice} {prefijo}", new Pais($"Pais {indice} {prefijo}"));
            contexto.Actividades.Add(new Actividad($"Paseo {indice}", "", 10m, 60, new Destino(ciudad)));
        }

        await contexto.SaveChangesAsync();
        await using var lectura = fixture.CrearContexto();

        await VerificarAsync(new PaisRepository(lectura).GetPaginaAsync, item => item.Id);
        await VerificarAsync(new CiudadRepository(lectura).GetPaginaAsync, item => item.Id);
        await VerificarAsync(new DestinoRepository(lectura).GetPaginaAsync, item => item.Id);
        await VerificarAsync(new ActividadRepository(lectura).GetPaginaAsync, item => item.Id);

        var empresaId = await CrearEmpresaConClientesAsync(1);
        await using var contextoEmpresa = fixture.CrearContextoConEmpresa(empresaId);
        var facturas = await new FacturaRepository(contextoEmpresa).GetPaginaAsync(new PaginacionRequest());
        Assert.Equal((0, 1), (facturas.Total, facturas.NumeroPagina));
    }

    private static async Task VerificarAsync<T>(
        Func<PaginacionRequest, Task<Pagina<T>>> obtener,
        Func<T, int> id)
    {
        var primera = await obtener(new PaginacionRequest());
        var segunda = await obtener(new PaginacionRequest { Pagina = 2 });

        Assert.Equal(10, primera.Elementos.Count);
        Assert.True(primera.Total >= 11);
        Assert.NotEmpty(segunda.Elementos);
        Assert.Empty(primera.Elementos.Select(id).Intersect(segunda.Elementos.Select(id)));
    }

    private async Task<int> CrearEmpresaConClientesAsync(int cantidad)
    {
        await using var contexto = fixture.CrearContexto();
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        var empresa = new EmpresaEntidad($"Agencia {sufijo}", $"CUIT-{sufijo}", "1");
        var clientes = Enumerable.Range(1, cantidad)
            .Select(numero => new Cliente($"Cliente {numero:00}", sufijo, $"{numero}-{sufijo}@c.com", "1"))
            .ToList();
        contexto.Empresas.Add(empresa);
        contexto.Clientes.AddRange(clientes);
        await contexto.SaveChangesAsync();
        contexto.EmpresaClientes.AddRange(clientes.Select(cliente => new EmpresaCliente(empresa.Id, cliente.Id)));
        await contexto.SaveChangesAsync();

        return empresa.Id;
    }
}
