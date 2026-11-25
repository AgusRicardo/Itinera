using System.ComponentModel.DataAnnotations;

namespace Itinera.Application.Common.Validaciones;

public sealed class TelefonoAttribute : RegularExpressionAttribute
{
    public TelefonoAttribute() : base(@"(?=([^0-9]*[0-9]){6})[0-9 ()+./-]+")
        => ErrorMessage = "Ingresá un teléfono válido, con al menos 6 números.";
}
