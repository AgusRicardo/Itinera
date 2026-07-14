using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Itinera.Application.Seguridad.Common;
using Itinera.Application.Seguridad.Dtos;
using Itinera.Application.Seguridad.Interfaces;
using Itinera.Domain.Seguridad;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Itinera.Application.Seguridad.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ISecurityRepository _repository;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(ISecurityRepository repository, IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _repository.GetByEmailAsync(request.Email);

        if (usuario is null || !PasswordHasher.Verify(request.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("La cuenta está desactivada.");

        var roles = await _repository.GetRolesDeUsuarioAsync(usuario.Id);
        var rolesNombres = roles.Select(r => r.Nombre).ToList();
        var token = GenerarToken(usuario, rolesNombres);

        return new LoginResponse(
            token,
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            rolesNombres);
    }

    public async Task<Guid> RegistrarAsync(RegistroRequest request)
    {
        var existente = await _repository.GetByEmailAsync(request.Email);
        if (existente is not null)
            throw new InvalidOperationException("El email ya está registrado.");

        var hash = PasswordHasher.Hash(request.Password);
        var usuario = new Usuario(request.Nombre, request.Email, hash);

        return await _repository.CreateUsuarioAsync(usuario);
    }

    private string GenerarToken(Usuario usuario, List<string> roles)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name, usuario.Nombre),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(rol =>
            new Claim(ClaimTypes.Role, rol)));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public UsuarioInfoResponse ObtenerInfoUsuario(ClaimsPrincipal user)
    {
        var usuarioId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var email = user.FindFirst(ClaimTypes.Email)!.Value;
        var nombre = user.FindFirst(ClaimTypes.Name)!.Value;
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return new UsuarioInfoResponse(usuarioId, email, nombre, roles);
    }
}
