using Itinera.Api;
using Itinera.Api.Middleware;
using Itinera.Infrastructure.Persistence;
using Itinera.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Render inyecta la variable PORT; la API escucha ahí para que el proxy
// del borde (que expone HTTPS) pueda alcanzarla.
var puerto = builder.Configuration["PORT"];
if (!string.IsNullOrWhiteSpace(puerto))
{
    builder.WebHost.UseUrls($"http://+:{puerto}");
}

builder.Services.AgregarItinera(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<SecuritySeeder>();
    await seeder.SembrarAsync();
}

// Configure the HTTP request pipeline.
var swaggerHabilitado = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerHabilitado)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

// Render termina el TLS en su borde y reenvía las cabeceras; sin esto,
// UseHttpsRedirection provocaría un loop de redirecciones en producción.
var opcionesForwarded = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
opcionesForwarded.KnownIPNetworks.Clear();
opcionesForwarded.KnownProxies.Clear();

app.UseForwardedHeaders(opcionesForwarded);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
