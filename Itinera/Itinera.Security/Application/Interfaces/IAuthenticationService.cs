using Itinera.Security.Application.Dtos;
using System.Security.Claims;

namespace Itinera.Security.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    UsuarioInfoResponse ObtenerInfoUsuario(ClaimsPrincipal user);
}
