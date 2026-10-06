using Itinera.Security.Application.Common;
using Xunit;

namespace Itinera.UnitTests.Seguridad;

public class PasswordHasherTests
{
    [Fact]
    public void Verificar_debe_aceptar_la_contrasena_original()
    {
        const string contrasena = "Itinera123!";
        var hash = PasswordHasher.Hash(contrasena);

        Assert.True(PasswordHasher.Verify(contrasena, hash));
        Assert.False(PasswordHasher.Verify("OtraContrasena", hash));
    }
}
