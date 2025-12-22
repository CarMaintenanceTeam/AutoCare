# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

AutoCare is a full-stack car maintenance booking platform connecting customers with service centers. The backend uses .NET 9 with Clean Architecture and CQRS, while the frontend is built with React 19.

## Architecture

### Backend - Clean Architecture Layers

The server follows strict Clean Architecture with clear dependency flow:

**Dependency Direction**: Api → Application → Domain ← Infrastructure

#### AutoCare.Domain (Core)
- Pure business logic with no external dependencies
- Contains: Entities, Value Objects, Enums, Domain Events
- Key entities: User, Customer, Employee, Vehicle, Booking, Service, ServiceCenter
- Domain events in `Events/` folder (e.g., BookingCreated, UserRegistered)

#### AutoCare.Application (Business Logic)
- Implements CQRS pattern using MediatR
- Structure: `Features/{FeatureName}/Commands|Queries/{OperationName}/`
- Each operation has: Command/Query, Handler, Validator, Response DTOs
- **Critical**: Handlers implement `ICommandHandler<TCommand, TResponse>` or `IQueryHandler<TQuery, TResponse>` interfaces
- Validation: FluentValidation validators in each feature folder
- MediatR Behaviors (applied via pipeline):
  - `ValidationBehavior` - Auto-validates requests before handler execution
  - `LoggingBehavior` - Logs requests/responses
  - `PerformanceBehavior` - Tracks execution time
- Interfaces for infrastructure concerns (IApplicationDbContext, IPasswordHasher, IJwtTokenService)

#### AutoCare.Infrastructure (External Concerns)
- EF Core DbContext with entity configurations in `Data/Configurations/`
- Data seeding in `Data/Seed/DataSeeder.cs` (creates test accounts on startup)
- JWT implementation: `Identity/JwtTokenService.cs` and `Identity/JwtSettings.cs`
- Password hashing with BCrypt.Net

#### AutoCare.Api (Entry Point)
- Thin controllers that delegate to MediatR
- All responses wrapped in `ApiResponse<T>` for consistency
- Global exception handling via `UseGlobalExceptionHandler()` middleware
- JWT Bearer authentication with Swagger integration
- CORS configured for React app (ports 3000, 5173, 5174)
- Swagger UI accessible at root URL in development

### Frontend - React Architecture

- **React 19** with React Router for navigation
- **Bootstrap 5** for styling (not Tailwind as README stated)
- Component structure in `src/Components/`
- Uses react-scripts (Create React App) build system

## Common Development Commands

### Backend (.NET)

#### Database Management
All EF Core commands must be run from `server/src/AutoCare.Api/` directory:

```powershell
# Apply pending migrations
dotnet ef database update --project ../AutoCare.Infrastructure --startup-project .

# Create new migration
dotnet ef migrations add MigrationName --project ../AutoCare.Infrastructure --startup-project .

# Reset database completely
dotnet ef database drop --project ../AutoCare.Infrastructure --startup-project . --force
dotnet ef database update --project ../AutoCare.Infrastructure --startup-project .

# Remove last migration (before applying it)
dotnet ef migrations remove --project ../AutoCare.Infrastructure --startup-project .
```

#### Build and Run

```powershell
# From server/ directory or AutoCare.sln location
dotnet restore
dotnet build

# Run API (from server/src/AutoCare.Api/)
dotnet run

# Run with watch mode
dotnet watch run
```

The API starts on:
- HTTPS: https://localhost:7146
- HTTP: http://localhost:5088
- Swagger UI: https://localhost:7146 (root path)

#### Testing
No test projects exist yet. When adding tests, follow the pattern:
- Create `server/tests/` directory
- Use xUnit for testing framework
- Structure: `AutoCare.Application.Tests`, `AutoCare.Domain.Tests`, etc.

### Frontend (React)

```powershell
# From client/ directory
npm install

# Start development server
npm start  # Runs on http://localhost:3000

# Build for production
npm build

# Run tests
npm test
```

## Key Configuration Files

### Backend
- `server/src/AutoCare.Api/appsettings.json` - Connection string, JWT settings, logging
- `server/src/AutoCare.Api/Program.cs` - Startup configuration, middleware pipeline
- JWT Configuration:
  - Key, Issuer, Audience in appsettings.json
  - AccessTokenExpirationMinutes: 15
  - RefreshTokenExpirationDays: 7

### Frontend
- `client/package.json` - Dependencies and scripts
- API base URL must be configured in client code to match backend (typically https://localhost:7146/api)

## Important Implementation Patterns

### Adding New Features (Backend)

1. **Create Domain Entity** (if needed) in `AutoCare.Domain/Entities/`
2. **Add Feature Folder** in `AutoCare.Application/Features/FeatureName/`
3. **For Commands** (mutations):
   - Create `Commands/OperationName/OperationCommand.cs` (implements `ICommand<TResponse>`)
   - Create `OperationCommandHandler.cs` (implements `ICommandHandler<OperationCommand, TResponse>`)
   - Create `OperationCommandValidator.cs` (inherits `AbstractValidator<OperationCommand>`)
4. **For Queries** (reads):
   - Create `Queries/OperationName/OperationQuery.cs` (implements `IQuery<TResponse>`)
   - Create `OperationQueryHandler.cs` (implements `IQueryHandler<OperationQuery, TResponse>`)
5. **Add Controller Endpoint** in `AutoCare.Api/Controllers/`
   - Inject `IMediator` via constructor
   - Send command/query: `await _mediator.Send(command, cancellationToken)`
   - Wrap response: `ApiResponse<T>.SuccessResponse(result)`
6. **EF Core Configuration** (if new entity):
   - Add entity to DbContext as `DbSet<TEntity>`
   - Create configuration in `AutoCare.Infrastructure/Data/Configurations/EntityConfiguration.cs`

### Authentication Flow

- Registration creates Customer users via `POST /api/auth/register`
- Login returns both access token (15 min) and refresh token (7 days)
- Refresh token rotation implemented for security
- Controllers use `[Authorize]` attribute for protected endpoints
- Test accounts seeded on first run (see README.md for credentials)

### Response Structure

All API responses follow this format:
```json
{
  "success": true,
  "data": { ... },
  "errors": null
}
```

### Database Seeding

- Automatic seeding on application startup via `DataSeeder.SeedAllAsync()`
- Seeds: Users, Customers, Employees, ServiceCenters, Services, Vehicles, Bookings
- Default admin: admin@autocare.com / Password123@

## Development Workflow Notes

- **Migrations**: Database migrations auto-apply on startup (see Program.cs line 168)
- **Logging**: Serilog writes to console and `logs/AutoCareLog-{Date}.log`
- **Validation**: FluentValidation runs automatically via ValidationBehavior before handler execution
- **CORS**: React app origins already configured (localhost:3000, 5173, 5174)
- **Connection String**: Update `appsettings.json` for your SQL Server instance
- **JWT Secret**: Current key in appsettings.json is for development only; change for production

## Known Patterns to Follow

- Controllers must be thin - only handle HTTP concerns, delegate to MediatR
- Handlers contain all business logic
- Use `IApplicationDbContext` abstraction, not concrete DbContext
- All commands/queries validated via FluentValidation before reaching handler
- Domain events not yet wired up with handlers (infrastructure exists but no subscribers)
- Service center locations use Latitude/Longitude for nearby searches
