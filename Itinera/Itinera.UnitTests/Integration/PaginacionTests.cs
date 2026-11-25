using Itinera.Application.Actividades.Dtos;
using Itinera.Application.Clientes.Mappers;
using Itinera.Application.Common.Exceptions;
using Itinera.Application.Conceptos.Dtos;
using Itinera.Application.Conceptos.Services;
using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Paginacion;
using Itinera.Application.Propuestas.Dtos;
using Itinera.Domain.Common;
using Itinera.Domain.Empresa;
using Itinera.Domain.Propuestas;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence.Repositories.Dominio;
using Itinera.Infrastructure.Persistence.Repositories.Seguridad;
using Itinera.Infrastructure.Servicios;
using Itinera.Security.Application.Services;
using Microsoft.EntityFrameworkCore;
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

        var primera = await repositorio.GetPaginaAsync(new ListadoActivableRequest());
        var segunda = await repositorio.GetPaginaAsync(new ListadoActivableRequest { Pagina = 2 });
        var fueraDeRango = await repositorio.GetPaginaAsync(new ListadoActivableRequest { Pagina = 99 });
        var filtrada = await repositorio.GetPaginaAsync(new ListadoActivableRequest { Texto = "CLIENTE 07", Activo = true });
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

        var primera = await repositorio.GetPaginaAsync(new ListadoComercialRequest());
        var segunda = await repositorio.GetPaginaAsync(new ListadoComercialRequest { Pagina = 2, Texto = "zoe u" });
        var presentadas = await repositorio.GetPaginaAsync(
            new ListadoComercialRequest { EstadoIds = [(int)EstadoPropuesta.Presentada] });
        var resumen = await repositorio.GetResumenAsync();

        Assert.Equal((10, 11), (primera.Elementos.Count, primera.Total));
        Assert.Single(segunda.Elementos);
        Assert.Empty(presentadas.Elementos);
        Assert.Equal((11, 11, 0), (resumen.Total, resumen.Borradores, resumen.Aceptadas));
    }

    [Theory]
    [InlineData("propuesta")]
    [InlineData("fechaCreacion")]
    [InlineData("cliente")]
    [InlineData("destino")]
    [InlineData("fechas")]
    [InlineData("presupuesto")]
    [InlineData("estado")]
    [InlineData("numero")]
    [InlineData("emision")]
    [InlineData("vencimiento")]
    [InlineData("total")]
    [InlineData("saldo")]
    [InlineData("contacto")]
    [InlineData("propuestas")]
    [InlineData("ultimaGestion")]
    public async Task Orden_SeTraduceASqlEnTodasLasColumnas(string orden)
    {
        var empresaId = await CrearEmpresaConClientesAsync(2);
        await using var contexto = fixture.CrearContextoConEmpresa(empresaId);
        var filtro = new ListadoComercialRequest
        {
            Orden = orden,
            Descendente = true,
            Texto = "a",
            ClienteId = 1,
            EstadoIds = [1, 2],
            Desde = DateTimeOffset.UtcNow.AddDays(-1),
            Hasta = DateTimeOffset.UtcNow
        };

        Assert.Empty((await new PropuestaRepository(contexto).GetPaginaAsync(filtro)).Elementos);
        Assert.Empty((await new FacturaRepository(contexto).GetPaginaAsync(filtro)).Elementos);
        Assert.Equal(2, (await new ClienteRepository(contexto, new TenantContextFake { EmpresaId = empresaId })
            .GetPaginaAsync(new ListadoActivableRequest { Orden = orden, Descendente = true })).Total);
    }

    [Fact]
    public async Task Propuestas_OrdenaYFiltraPorFechaYCliente_YElClienteMuestraSuGestion()
    {
        var empresaId = await CrearEmpresaConClientesAsync(1);
        int clienteId;
        await using (var contexto = fixture.CrearContexto())
        {
            var empresa = await contexto.Empresas.FindAsync(empresaId);
            var empleado = new Empleado("Ana", "Gomez", $"{Guid.NewGuid():N}@e.com", "1", new Cargo("Agente"), empresa!);
            var cliente = new Cliente("Zoe", "Unica", $"{Guid.NewGuid():N}@c.com", "1");
            contexto.AddRange(empleado, cliente);
            contexto.Propuestas.AddRange(
                new Propuesta(cliente, empleado, 500),
                new Propuesta(cliente, empleado, 900),
                new Propuesta(cliente, empleado, 700));
            await contexto.SaveChangesAsync();
            clienteId = cliente.Id;
            contexto.EmpresaClientes.Add(new EmpresaCliente(empresaId, clienteId));
            await contexto.SaveChangesAsync();
        }

        await using var lectura = fixture.CrearContextoConEmpresa(empresaId);
        var repositorio = new PropuestaRepository(lectura);
        var manana = DateTimeOffset.UtcNow.AddDays(1);

        var ordenadas = await repositorio.GetPaginaAsync(new ListadoComercialRequest { Orden = "presupuesto" });
        var delCliente = await repositorio.GetPaginaAsync(
            new ListadoComercialRequest { ClienteId = clienteId, EstadoIds = [1], Hasta = manana });
        var futuras = await repositorio.GetPaginaAsync(new ListadoComercialRequest { Desde = manana });
        var clientes = await new ClienteRepository(lectura, new TenantContextFake { EmpresaId = empresaId })
            .GetPaginaAsync(new ListadoActivableRequest { Orden = "propuestas", Descendente = true });
        var primero = ClienteMapper.ToResponse(clientes.Elementos[0]);

        Assert.Equal([500m, 700m, 900m], ordenadas.Elementos.Select(item => item.Presupuesto));
        Assert.Equal(3, delCliente.Total);
        Assert.Equal(0, futuras.Total);
        Assert.Equal((clienteId, 3), (primero.Id, primero.CantidadPropuestas));
        Assert.NotNull(primero.UltimaGestion);
    }

    [Fact]
    public async Task Un_cliente_inactivo_sigue_en_la_grilla_con_sus_propuestas_y_se_puede_reactivar()
    {
        var empresaId = await CrearEmpresaConClientesAsync(1);
        int clienteId;
        await using (var contexto = fixture.CrearContexto())
        {
            var empresa = await contexto.Empresas.FindAsync(empresaId);
            var empleado = new Empleado("Ana", "Gomez", $"{Guid.NewGuid():N}@e.com", "1", new Cargo("Agente"), empresa!);
            var cliente = new Cliente("Zoe", "Inactiva", $"{Guid.NewGuid():N}@c.com", "1");
            cliente.Desactivar();
            contexto.AddRange(empleado, cliente, new Propuesta(cliente, empleado, 500));
            await contexto.SaveChangesAsync();
            clienteId = cliente.Id;
            contexto.EmpresaClientes.Add(new EmpresaCliente(empresaId, clienteId));
            await contexto.SaveChangesAsync();
        }

        await using var lectura = fixture.CrearContextoConEmpresa(empresaId);
        var tenant = new TenantContextFake { EmpresaId = empresaId };
        var repositorio = new ClienteRepository(lectura, tenant);
        var ajeno = new ClienteRepository(lectura, new TenantContextFake { EmpresaId = empresaId + 1000 });

        Assert.Equal(2, (await repositorio.GetPaginaAsync(new ListadoActivableRequest())).Total);
        Assert.Equal(clienteId, Assert.Single(
            (await repositorio.GetPaginaAsync(new ListadoActivableRequest { Activo = false })).Elementos).Id);
        var resumen = await repositorio.GetResumenAsync();
        Assert.Equal((2, 1), (resumen.Total, resumen.Activos));
        Assert.Single(await repositorio.GetAllAsync());
        Assert.Null(await repositorio.GetByIdAsync(clienteId));
        Assert.Null(await ajeno.GetByIdConInactivosAsync(clienteId));
        Assert.Equal("Zoe", Assert.Single(
            (await new PropuestaRepository(lectura).GetPaginaAsync(new ListadoComercialRequest())).Elementos).Cliente.Nombre);

        var inactivo = await repositorio.GetByIdConInactivosAsync(clienteId);
        inactivo!.Activar();
        await repositorio.UpdateAsync(inactivo);
        Assert.Equal(2, (await repositorio.GetAllAsync()).Count);
    }

    [Fact]
    public async Task Conceptos_SonDeCadaEmpresaYNoSeRepiten()
    {
        var empresaId = await CrearEmpresaConClientesAsync(1);
        var otraEmpresaId = await CrearEmpresaConClientesAsync(1);
        await using var contexto = fixture.CrearContextoConEmpresa(empresaId);
        await using var otroContexto = fixture.CrearContextoConEmpresa(otraEmpresaId);
        var servicio = new ConceptoFacturaService(
            new ConceptoFacturaRepository(contexto), new TenantContextFake { EmpresaId = empresaId });
        var otroServicio = new ConceptoFacturaService(
            new ConceptoFacturaRepository(otroContexto), new TenantContextFake { EmpresaId = otraEmpresaId });
        var request = new CrearConceptoFacturaRequest { Descripcion = "Aéreos", Precio = 100 };

        var creado = await servicio.CrearAsync(request);
        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.CrearAsync(request));
        await otroServicio.CrearAsync(request);
        await Assert.ThrowsAsync<ConceptoFacturaNoEncontradoException>(() => otroServicio.EliminarAsync(creado.Id));
        await servicio.ActualizarAsync(creado.Id, new ActualizarConceptoFacturaRequest { Descripcion = "Aéreos", Precio = 150 });

        var pagina = await servicio.ObtenerPaginaAsync(new ListadoRequest { Texto = "aér", Orden = "precio" });
        Assert.Equal(150, Assert.Single(pagina.Elementos).Precio);

        await servicio.ActualizarAsync(creado.Id, new ActualizarConceptoFacturaRequest { Descripcion = "Aéreos", Porcentaje = 12.5m });
        Assert.Equal((0m, 12.5m), (await servicio.ObtenerPaginaAsync(new ListadoRequest { Orden = "precio", Descendente = true }))
            .Elementos.Select(item => (item.Precio, item.Porcentaje)).Single());

        await servicio.CrearAsync(new CrearConceptoFacturaRequest { Descripcion = "Hotel", Precio = 900 });
        var porPorcentaje = await servicio.ObtenerPaginaAsync(new ListadoRequest { Orden = "porcentaje", Descendente = true });
        var porPrecio = await servicio.ObtenerPaginaAsync(new ListadoRequest { Orden = "precio", Descendente = true });
        Assert.Equal(["Aéreos", "Hotel"], porPorcentaje.Elementos.Select(item => item.Descripcion));
        Assert.Equal(["Hotel", "Aéreos"], porPrecio.Elementos.Select(item => item.Descripcion));
        await servicio.EliminarAsync(porPrecio.Elementos[0].Id);

        await servicio.EliminarAsync(creado.Id);
        Assert.Empty(await servicio.ObtenerTodosAsync());
        await servicio.CrearAsync(request);
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

        await VerificarAsync<Pais, ListadoRequest>(
            new PaisRepository(lectura).GetPaginaAsync, item => item.Id, $"Pais 3 {prefijo}");
        await VerificarAsync<Ciudad, ListadoRequest>(
            new CiudadRepository(lectura).GetPaginaAsync, item => item.Id, $"Pais 3 {prefijo}");
        await VerificarAsync<Destino, ListadoRequest>(
            new DestinoRepository(lectura).GetPaginaAsync, item => item.Id, $"Pais 3 {prefijo}");
        await VerificarAsync<Actividad, ListarActividadesRequest>(
            new ActividadRepository(lectura).GetPaginaAsync, item => item.Id, $"Pais 3 {prefijo}");

        var actividad = (await new ActividadRepository(lectura).GetPaginaAsync(
            new ListarActividadesRequest { Texto = $"Pais 3 {prefijo}" })).Elementos.Single();
        Assert.Equal(1, (await new ActividadRepository(lectura).GetPaginaAsync(
            new ListarActividadesRequest { DestinoId = actividad.DestinoId, GeneradoPorIA = false })).Total);
        Assert.Equal(0, (await new ActividadRepository(lectura).GetPaginaAsync(
            new ListarActividadesRequest { DestinoId = actividad.DestinoId, GeneradoPorIA = true })).Total);

        var empresaId = await CrearEmpresaConClientesAsync(1);
        await using var contextoEmpresa = fixture.CrearContextoConEmpresa(empresaId);
        var facturas = await new FacturaRepository(contextoEmpresa).GetPaginaAsync(new ListadoComercialRequest());
        Assert.Equal((0, 1), (facturas.Total, facturas.NumeroPagina));
    }

    [Theory]
    [InlineData("razonSocial")]
    [InlineData("cuit")]
    [InlineData("telefono")]
    [InlineData("estado")]
    [InlineData("nombre")]
    [InlineData("email")]
    [InlineData("empresa")]
    [InlineData("pais")]
    [InlineData("ciudad")]
    [InlineData("destino")]
    [InlineData("costo")]
    public async Task Orden_SeTraduceASqlEnCatalogosYAdministracion(string orden)
    {
        await using var contexto = fixture.CrearContexto();
        var tenant = new TenantContextFake { EsAdministradorPlataforma = true };
        var filtro = new ListadoActivableRequest { Orden = orden, Descendente = true, Texto = "a", Activo = true };
        var usuarios = new UsuarioPaginadoService(
            contexto, tenant, new UsuarioAdminService(new SecurityRepository(contexto, tenant)));

        await new EmpresaRepository(contexto).GetPaginaAsync(filtro);
        await usuarios.ObtenerPaginaAsync(filtro);
        await new PaisRepository(contexto).GetPaginaAsync(filtro);
        await new CiudadRepository(contexto).GetPaginaAsync(filtro);
        await new DestinoRepository(contexto).GetPaginaAsync(filtro);
        await new ActividadRepository(contexto).GetPaginaAsync(
            new ListarActividadesRequest { Orden = orden, Texto = "a" });
    }

    [Fact]
    public async Task Busqueda_IgnoraTildesYMayusculas_YElCambioQuedaAuditado()
    {
        var sufijo = Guid.NewGuid().ToString("N")[..8];
        await using var contexto = fixture.CrearContexto();
        var pais = new Pais($"Perú {sufijo}");
        contexto.Paises.Add(pais);
        await contexto.SaveChangesAsync();
        pais.ActualizarNombre($"Perú Andino {sufijo}");
        await contexto.SaveChangesAsync();
        pais.Desactivar();
        await contexto.SaveChangesAsync();
        pais.Activar();
        await contexto.SaveChangesAsync();

        var pagina = await new PaisRepository(contexto).GetPaginaAsync(
            new ListadoRequest { Texto = $"PERU andino {sufijo}" });
        var acciones = await contexto.RegistrosAuditoria
            .Where(registro => registro.Entidad == "Pais" && registro.EntidadId == pais.Id.ToString())
            .OrderBy(registro => registro.Id)
            .Select(registro => registro.Accion)
            .ToListAsync();

        Assert.Equal(pais.Id, Assert.Single(pagina.Elementos).Id);
        Assert.Equal(["Crear", "Modificar", "Desactivar", "Activar"], acciones);
    }

    private static async Task VerificarAsync<T, TFiltro>(
        Func<TFiltro, Task<Pagina<T>>> obtener,
        Func<T, int> id,
        string textoUnico)
        where TFiltro : ListadoRequest, new()
    {
        var primera = await obtener(new TFiltro());
        var segunda = await obtener(new TFiltro { Pagina = 2 });
        var buscada = await obtener(new TFiltro { Texto = textoUnico.ToUpperInvariant() });

        Assert.Equal(10, primera.Elementos.Count);
        Assert.True(primera.Total >= 11);
        Assert.NotEmpty(segunda.Elementos);
        Assert.Empty(primera.Elementos.Select(id).Intersect(segunda.Elementos.Select(id)));
        Assert.Equal(1, buscada.Total);
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
