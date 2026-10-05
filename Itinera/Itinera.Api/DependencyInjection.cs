using System.Text;
using Itinera.Api.Security;
using Itinera.Application;
using Itinera.Application.Common.Interfaces;
using Itinera.Infrastructure;
using Itinera.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Itinera.Api;

public static class DependencyInjection
{
    public static IServiceCollection AgregarItinera(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        servicios.AddControllers();
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
        var jwtKey = configuracion["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");

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

        servicios.AddAuthorization();
    }
}
