using System.Text;
using System.Text.Json.Serialization;
using Itinera.Api.Security;
using Itinera.Application;
using Itinera.Application.Common.Interfaces;
using Itinera.Infrastructure;
using Itinera.Security;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Itinera.Api;

public static class DependencyInjection
{
    public static IServiceCollection AgregarItinera(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        servicios.AddControllers()
            .AddJsonOptions(opciones =>
                opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        servicios.AddEndpointsApiExplorer();
        servicios.AddSwaggerGen();
        servicios.AddHttpContextAccessor();
        servicios.AddScoped<ICurrentUserService, CurrentUserService>();

        AgregarAutenticacion(servicios, configuracion);

        servicios.AddApplication();
        servicios.AddInfrastructure(configuracion);
        servicios.AddSecurity(configuracion);

        return servicios;
    }

    private static void AgregarAutenticacion(
        IServiceCollection servicios,
        IConfiguration configuracion)
    {
        var jwtKey = configuracion["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT Key is not configured. Configure 'Jwt:Key' via user-secrets or environment variables.");
        }

        servicios.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones =>
            {
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuracion["Jwt:Issuer"],
                    ValidAudience = configuracion["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),
                };
            });

        servicios.AddSingleton<IAuthorizationHandler, PermisoAuthorizationHandler>();

        servicios.AddAuthorization(opciones =>
        {
            foreach (var definicion in Permisos.Catalogo)
            {
                opciones.AddPolicy(definicion.Codigo, politica =>
                    politica
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermisoRequirement(definicion.Codigo)));
            }
        });
    }
}
