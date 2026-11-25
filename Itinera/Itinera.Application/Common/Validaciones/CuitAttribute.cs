using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Common.Validaciones;

public sealed class CuitAttribute : RegularExpressionAttribute
{
    public CuitAttribute() : base(@"[0-9]{2}-?[0-9]{8}-?[0-9]")
        => ErrorMessage = "Ingresá un CUIT válido: 11 números, con o sin guiones.";

    public static string SoloNumeros(string cuit) => cuit.Replace("-", string.Empty).Trim();
}
