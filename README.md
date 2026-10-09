# Itinera

Plataforma de gestión de viajes para agencias de turismo. Permite administrar
clientes, propuestas de viaje e itinerarios personalizados desde un único sistema.

Este repositorio contiene, por el momento, únicamente el backend.

## Tecnologías utilizadas

| Área | Tecnología | Versión |
| --- | --- | --- |
| Plataforma | .NET | 10.0 |
| API | ASP.NET Core Web API | 10.0 |
| Acceso a datos | Entity Framework Core | 10.0.9 |
| Proveedor de base de datos | Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0 |
| Base de datos | PostgreSQL | 17 |
| Autenticación | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) | 10.0.9 |
| Documentación de API | Swashbuckle (Swagger) | 10.2.3 |
| Pruebas unitarias | xUnit | 2.9.3 |
| Contenedores | Docker / Docker Compose | - |

## Arquitectura

El backend sigue una arquitectura en capas con separación de responsabilidades:

- **Itinera.Domain**: entidades y reglas de negocio.
- **Itinera.Application**: casos de uso, servicios, interfaces y DTOs.
- **Itinera.Infrastructure**: persistencia con EF Core, repositorios y migraciones.
- **Itinera.Security**: módulo de seguridad (usuarios, roles, permisos, JWT),
  desacoplado del dominio.
- **Itinera.Api**: capa de presentación (controllers, middleware, configuración).
- **Itinera.UnitTests**: pruebas unitarias con xUnit.

## Estructura de la solución

```
Itinera/
├── Itinera.Api/
├── Itinera.Application/
├── Itinera.Domain/
├── Itinera.Infrastructure/
├── Itinera.Security/
├── Itinera.UnitTests/
├── docker-compose.yml
└── Itinera.slnx
```

## Requisitos previos

- .NET SDK 10
- Docker y Docker Compose
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef`

## Puesta en marcha (desarrollo)

### Día a día (lo más común)

1. Levantar la base de datos (una vez por sesión):

   ```powershell
   docker compose up -d postgres
   ```

2. En Visual Studio, elegir el perfil **`https`** en el desplegable de Run y **F5**.
   - Corre la API en tu PC (`https://localhost:7164`) con tus user-secrets y Swagger.
   - Alternativa por consola: `dotnet run --project Itinera.Api`.

3. Al terminar: `docker compose stop postgres`.

> No uses el perfil "Container (Dockerfile)": dentro del contenedor la API no alcanza
> la base por `localhost`. Para levantar todo dockerizado, ver
> "Alternativa: todo con Docker Compose".

### Configuración inicial (una sola vez)

1. Configurar los secretos locales con user-secrets:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=itinera;Username=itinera;Password=<password>;" --project Itinera.Api
   dotnet user-secrets set "Jwt:Key" "<clave_de_al_menos_32_caracteres>" --project Itinera.Api
   ```

2. Copiar `.env.example` a `.env` y completar los valores para Docker Compose.

3. Levantar PostgreSQL:

   ```powershell
   docker compose up -d postgres
   ```

4. Aplicar las migraciones:

   ```powershell
   dotnet ef database update --project Itinera.Infrastructure --startup-project Itinera.Api
   ```

5. Ejecutar la API:

   ```powershell
   dotnet run --project Itinera.Api
   ```

### Alternativa: todo con Docker Compose

Para levantar **PostgreSQL + API** juntos en contenedores:

```powershell
docker compose up -d --build
```

- La API queda en `http://localhost:8080`.
- `docker compose up -d postgres` levanta solo la base (para correr la API con
  `dotnet run` en tu máquina).
- `docker compose down` detiene y elimina los contenedores (conserva los datos).
- `docker compose down -v` elimina también el volumen (borra los datos).

En Compose la API corre con `ASPNETCORE_ENVIRONMENT=Production`, así que Swagger no
está disponible. El connection string, `Jwt:Key` y el admin inicial se toman de `.env`
(por eso la API del contenedor se conecta a `Host=postgres`).

> Nota: el perfil "Container (Dockerfile)" de Visual Studio crea un contenedor aparte
> que usa `localhost` como host de la base y no puede conectarse. Para Docker, usá
> Compose.

## Pruebas

Todos los tests (unitarios, de arquitectura y de integración):

```powershell
dotnet test
```

Solo los tests rápidos, sin base de datos:

```powershell
dotnet test --filter "Category!=Integration"
```

### Tests de integración

Requieren un PostgreSQL accesible. Por defecto usan
`Host=localhost;Port=5432;Database=itinera_test;Username=itinera;Password=itinera_dev`,
que coincide con el servicio de `docker-compose.yml`.

```powershell
docker compose up -d postgres
dotnet test --filter "Category=Integration"
```

Para apuntar a otra base, definir la variable de entorno:

```powershell
$env:ITINERA_TEST_CONNECTION = "Host=localhost;Port=5432;Database=itinera_test;Username=itinera;Password=itinera_dev"
```

> La base de tests se recrea en cada corrida (`EnsureDeleted` + `Migrate`).

## Seguridad

- Autenticación por JWT. Los permisos efectivos viajan como claims `permiso`.
- Roles: `Administrador` (todos los permisos) y `Agente` (operación de clientes,
  propuestas, itinerarios y facturación).
- Al iniciar, un seeder idempotente crea los permisos y roles, y un usuario
  administrador inicial a partir de `Seed:Admin:*` (solo si no hay usuarios).
- Endpoints de administración protegidos con `seguridad.gestionar`:
  `/api/usuarios`, `/api/roles`, `/api/grupos`.
- Los grupos son un **Composite**: pueden contener usuarios y otros grupos, y los
  roles se heredan de todos los grupos contenedores.

## Secretos y configuración

Los secretos no se versionan. Según el entorno se resuelven así:

- **Local (`dotnet run`)**: user-secrets.
- **Local (Docker Compose)**: archivo `.env` (ignorado por Git).
- **Producción**: variables de entorno o `.env` en el servidor.

El archivo `.env.example` documenta las claves necesarias y sí se versiona.
