# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution Overview

MojMajstor-Dashboard is a .NET 8 ASP.NET Core dashboard application (admin panel + REST API) for managing an e-commerce/services platform. It uses a hybrid MVC + Web API architecture with SQL Server and Entity Framework Core.

## Commands

```bash
# Build the entire solution
dotnet build Universal.sln

# Run the web application (startup project is Universal)
dotnet run --project Universal

# Apply EF Core migrations (run from solution root)
dotnet ef database update --project Entities --startup-project Universal

# Add a new migration
dotnet ef migrations add <MigrationName> --project Entities --startup-project Universal
```

Swagger UI is available at `/swagger` in Development mode. Default route lands on `Authentication/Index`.

## Project Structure

| Project | Role |
|---|---|
| `Universal` | ASP.NET Core web host — MVC views + API controllers, `Program.cs`, static files |
| `Entities` | EF Core DbContexts, entity models, migrations, AutoMapper profile |
| `Services` | Business logic services, JWT auth, AWS S3, email, PDF, Excel |
| `UniversalDTO` | Input DTOs (`IDTO/`), Output DTOs (`ODTO/`), View DTOs (`ViewDTO/`) |
| `UniversalMapping` | Currently unused placeholder |

## Architecture & Key Patterns

### Two Databases / Two Contexts
- `MainContext` — primary application database (products, orders, users, media, etc.)
- `MojMajstorContext` — secondary database for MojMajstor-specific data (connection string is hardcoded in `Program.cs` for now)
- `BaseServices` exposes both as `_context` and `_context2`, plus `_mapper` and `_usersServices`
- Use `SaveContextChangesAsync()` / `SaveContextChangesMajstorAsync()` from `BaseServices` instead of calling `SaveChangesAsync()` directly — they handle transaction rollback on `DbUpdateException`

### Service Layer
All business logic lives in services that inherit `BaseServices`. There are only two registered services:
- `MainDataServices` (scoped) — products, categories, orders, media, dashboard data
- `UsersServices` (scoped) — user auth, JWT generation, BCrypt password hashing

Add new domain methods to one of these services rather than creating many small ones.

### Controllers
- `Universal/Admin-Controllers/AdminMVC/` — MVC controllers returning Razor views (`DashboardController`, `AuthenticationController`)
- `Universal/Admin-Controllers/AdminAPI/` — REST API controllers returning JSON (all under the same folder, one per entity)

### Authentication & Authorization
JWT-based auth via custom middleware (`Services/Authorization/JwtMiddleware.cs`):
1. Middleware extracts Bearer token from `Authorization` header
2. `JwtUtils.ValidateJwtToken()` validates it and returns `userId`
3. Validated user object is attached to `HttpContext.Items["User"]`
4. Custom attributes gate access: `[Authorize]` (requires valid JWT), `[AllowAnonymous]`, `[RequiresAuthToken]`

Do **not** use ASP.NET Core's built-in `[Authorize]` from `Microsoft.AspNetCore.Authorization` — this project uses its own attribute from `Services.Authorization`.

### DTO Naming Convention
- `ProductIDTO` — input (client → server)
- `ProductODTO` — output (server → client)
- `ProductViewDTO` — used in MVC Razor views

AutoMapper profile is in `Entities/Mapping/UniversalMappingProfile.cs`. Register new mappings there.

### Configuration & Environment
Config is loaded in layers: `appsettings.json` → `.env` (via DotNetEnv) → environment variables.
A `.env` file must exist next to `Universal/` at runtime. Key config sections:
- `ConnectionStrings:MainDatabase` — primary SQL Server connection
- `AppSettings:Secret` — JWT signing secret
- `ServiceConfiguration:AWSS3` — AWS S3 credentials and bucket name
- `EmailSettings` — Azure OAuth2/MailKit email config
- `Jwt:Key` / `Jwt:Issuer` — JWT parameters

## Dependencies Worth Knowing

- **AutoMapper 13** — mapping between entities and DTOs
- **BCrypt.Net-Next** — password hashing in `UsersServices`
- **AWSSDK.S3** — file storage via `AWSS3FileService` / `AWSS3BucketHelper` (both transient)
- **MailKit + MimeKit** — email sending via Azure OAuth2 in `Services/Helpers/MailService.cs`
- **IronPdf** — PDF generation
- **EPPlus** — Excel export
- **DotNetEnv** — loads `.env` file at startup
- **Chart.js 3.7** — frontend charting (served as static lib)
