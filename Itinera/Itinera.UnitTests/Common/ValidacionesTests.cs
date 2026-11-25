using System.ComponentModel.DataAnnotations;
using Itinera.Api.Validation;
using Itinera.Application.Clientes.Dtos;
using Itinera.Application.Common.Validaciones;
using Itinera.Security.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itinera.UnitTests.Common;

public class ValidacionesTests
{
    [Theory]
    [InlineData("", true)]
    [InlineData("2025550172", true)]
    [InlineData("+54 (11) 4555-1234", true)]
    [InlineData("abc", false)]
    [InlineData("   ", false)]
    [InlineData("12345", false)]
    [InlineData("+-()", false)]
    public void El_telefono_exige_numeros(string telefono, bool esValido)
        => Assert.Equal(esValido, new TelefonoAttribute().IsValid(telefono));

    [Theory]
    [InlineData("30-12345678-9", true)]
    [InlineData("30123456789", true)]
    [InlineData("abc", false)]
    [InlineData("30-1234567-9", false)]
    public void El_cuit_exige_11_numeros(string cuit, bool esValido)
        => Assert.Equal(esValido, new CuitAttribute().IsValid(cuit));

    [Theory]
    [InlineData(typeof(CrearClienteRequest), "Nombre", "El campo Nombre es obligatorio.")]
    [InlineData(typeof(CrearClienteRequest), "Email", "Ingresá un correo electrónico válido.")]
    [InlineData(typeof(CrearGrupoRequest), "Nombre", "El campo Nombre admite hasta 200 caracteres.")]
    public void Las_validaciones_responden_en_espanol(Type tipo, string campo, string esperado)
    {
        var servicios = new ServiceCollection();
        servicios.AddLogging();
        servicios.AddControllers(MensajesValidacionProvider.Configurar);
        var metadatos = servicios.BuildServiceProvider()
            .GetRequiredService<IModelMetadataProvider>()
            .GetMetadataForType(tipo);
        var campos = metadatos.BoundConstructor?.BoundConstructorParameters ?? metadatos.Properties;
        var mensajes = campos
            .Single(item => item.Name == campo)
            .ValidatorMetadata
            .OfType<ValidationAttribute>()
            .Select(validacion => validacion.FormatErrorMessage(campo));

        Assert.Contains(esperado, mensajes);
    }
}
