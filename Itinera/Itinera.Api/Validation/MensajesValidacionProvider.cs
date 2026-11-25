using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Itinera.Api.Validation;

public sealed class MensajesValidacionProvider : IValidationMetadataProvider
{
    public static void Configurar(MvcOptions opciones)
    {
        opciones.ModelMetadataDetailsProviders.Add(new MensajesValidacionProvider());

        var mensajes = opciones.ModelBindingMessageProvider;
        mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "Faltan los datos de la solicitud.");
        mensajes.SetMissingBindRequiredValueAccessor(campo => $"El campo {campo} es obligatorio.");
        mensajes.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
        mensajes.SetValueIsInvalidAccessor(valor => $"El valor {valor} no es válido.");
        mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor {valor} no es válido para {campo}.");
        mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor {valor} no es válido.");
        mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
    }

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        foreach (var validacion in context.ValidationMetadata.ValidatorMetadata.OfType<ValidationAttribute>())
        {
            // EmailAddress ya trae un mensaje propio en inglés.
            if (validacion.ErrorMessage is not null && validacion is not EmailAddressAttribute)
                continue;

            validacion.ErrorMessage = validacion switch
            {
                RequiredAttribute => "El campo {0} es obligatorio.",
                EmailAddressAttribute => "Ingresá un correo electrónico válido.",
                MaxLengthAttribute => "El campo {0} admite hasta {1} caracteres.",
                MinLengthAttribute => "El campo {0} debe tener al menos {1} elementos.",
                RangeAttribute => "El valor del campo {0} no está permitido.",
                _ => "El valor del campo {0} no es válido."
            };
        }
    }
}
