using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Itinera.Security.Application.Common;
using Itinera.Security.Application.Dtos;
using Itinera.Security.Application.Interfaces;
using Itinera.Security.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Itinera.Security.Application.Services;

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
        var permisos = await _repository.GetPermisosDeUsuarioAsync(usuario.Id);
        var permisosCodigos = permisos.Select(p => p.Codigo).ToList();
        var empresa = await _repository.GetEmpresaDeUsuarioAsync(usuario.Id);
        var token = GenerarToken(usuario, rolesNombres, permisosCodigos);

        return new LoginResponse(
            token,
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            rolesNombres,
            permisosCodigos,
            empresa.EmpresaRazonSocial ?? string.Empty);
    }

    private string GenerarToken(Usuario usuario, List<string> roles, List<string> permisos)
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

        claims.AddRange(permisos.Select(permiso =>
            new Claim(Permisos.ClaimType, permiso)));

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
        var usuarioId = Guid.Parse(
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new UnauthorizedAccessException("Token sin identificador de usuario."));

        var email = user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? string.Empty;

        var nombre = user.FindFirst(ClaimTypes.Name)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Name)?.Value
            ?? string.Empty;

        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var permisos = user.FindAll(Permisos.ClaimType).Select(c => c.Value).ToList();

        return new UsuarioInfoResponse(usuarioId, email, nombre, roles, permisos);
    }
}
