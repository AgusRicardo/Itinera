using Itinera.Security.Application.Dtos;
using System.Security.Claims;

namespace Itinera.Security.Aplicacion.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<Guid> RegistrarAsync(RegistroRequest request);
    UsuarioInfoResponse ObtenerInfoUsuario(ClaimsPrincipal user);
}
