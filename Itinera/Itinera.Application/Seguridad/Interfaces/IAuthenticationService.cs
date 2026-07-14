using System.Security.Claims;
using Itinera.Application.Seguridad.Dtos;

namespace Itinera.Application.Seguridad.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<Guid> RegistrarAsync(RegistroRequest request);
    UsuarioInfoResponse ObtenerInfoUsuario(ClaimsPrincipal user);
}
