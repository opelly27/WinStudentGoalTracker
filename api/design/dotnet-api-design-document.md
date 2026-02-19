# .NET API Project Design Document

A reference architecture for building ASP.NET Core Web APIs with Dapper, JWT authentication, and a co-located data access layer.

---

## Project Structure

All code lives in a single project. The data access layer is organized as a namespace within the same assembly rather than a separate project.

```
MyApi/
├── MyApi.sln
├── MyApi.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── Configuration/
│   └── (Options classes for external services)
├── DataAccess/
│   ├── DatabaseManager.cs
│   ├── Models/
│   │   ├── DatabaseObjects/
│   │   └── DataTransferObjects/
│   └── Repositories/
└── src/
    ├── BaseClasses/
    │   └── BaseController.cs
    ├── Controllers/
    ├── Middleware/
    ├── Models/
    │   └── ResponseTypes/
    └── Services/
```

---

## Layer Responsibilities

### Controllers (`src/Controllers/`)

Controllers handle HTTP concerns only: request validation, claims extraction, and response shaping. They delegate all business logic to services and all data access to repositories.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ExampleController : BaseController
{
    private readonly ExampleRepository _exampleRepository = new();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ResponseResult<ExampleResponse>>> GetById(Guid id)
    {
        var (userId, error) = GetUserIdFromClaims();
        if (error != null) return error;

        var entity = await _exampleRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        return Ok(new ResponseResult<ExampleResponse>
        {
            Success = true,
            Data = MapToResponse(entity)
        });
    }
}
```

**Conventions:**
- Use `[ApiController]` and `[Route("api/[controller]")]` on every controller
- Inherit from `BaseController` for claims extraction helpers
- Return `ActionResult<ResponseResult<T>>` for consistent response envelopes
- Apply `[Authorize]` or `[Authorize(Roles = "...")]` per-endpoint
- Use `[ProducesResponseType]` attributes for OpenAPI documentation

### Base Controller (`src/BaseClasses/BaseController.cs`)

Provides protected helper methods that extract and validate JWT claims, reducing boilerplate across controllers.

```csharp
public class BaseController : ControllerBase
{
    protected (Guid userId, ActionResult? error) GetUserIdFromClaims() { ... }
    protected (string email, List<string> roles, ActionResult? error) GetUserDetailsFromClaims() { ... }
    protected bool HasRole(string role) { ... }
    protected bool HasAnyRole(params string[] roles) { ... }
}
```

Add additional claim extraction helpers as needed for your domain (e.g., tenant ID, organization ID).

These methods return tuples with an optional error `ActionResult`, enabling early-return patterns in controller actions.

### Services (`src/Services/`)

Services encapsulate business logic that spans multiple repositories or involves non-trivial orchestration. Services are either static classes (for stateless logic) or singletons instantiated directly where needed.

**When to use a service vs. calling a repository directly from a controller:**
- **Direct repository call:** Simple CRUD with no cross-cutting logic
- **Service:** Multi-step operations, external API calls, or any logic spanning multiple repositories

**Static service pattern** (preferred for stateless logic):

```csharp
public static class MyService
{
    public static async Task<Result> DoSomethingAsync(Guid entityId)
    {
        var repo = new ExampleRepository();
        // orchestration logic
    }
}
```

**Singleton instance pattern** (when the service needs initialization or holds config):

```csharp
public class MyService
{
    public static MyService Instance { get; private set; } = null!;

    public static void Initialize(string apiKey, string baseUrl)
    {
        Instance = new MyService { _apiKey = apiKey, _baseUrl = baseUrl };
    }

    private string _apiKey;
    private string _baseUrl;

    public async Task<Result> DoSomethingAsync() { ... }
}

// In Program.cs
MyService.Initialize(
    builder.Configuration["MyService:ApiKey"]!,
    builder.Configuration["MyService:BaseUrl"]!);
```

### Repositories (`DataAccess/Repositories/`)

Each repository maps to a single domain entity and serves as a thin C# wrapper around stored procedures. Repositories create their own database connections per call and are responsible for calling stored procedures and assembling the results into typed objects.

**All query logic lives in stored procedures in the database — repositories contain no inline SQL.**

```csharp
public class ExampleRepository
{
    private IDbConnection Connection =>
        new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<dbExample?> GetByIdAsync(Guid id)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbExample>(
            "sp_Example_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dbExample>> GetByOwnerAsync(Guid ownerId)
    {
        using var db = Connection;
        return await db.QueryAsync<dbExample>(
            "sp_Example_GetByOwner",
            new { OwnerId = ownerId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> InsertAsync(dbExample entity)
    {
        using var db = Connection;
        var rows = await db.ExecuteAsync(
            "sp_Example_Insert",
            new { entity.Id, entity.Name, entity.CreatedAt },
            commandType: CommandType.StoredProcedure);
        return rows > 0;
    }

    /// For stored procedures that return multiple result sets,
    /// use QueryMultipleAsync to assemble a composite object.
    public async Task<ExampleWithDetails?> GetWithDetailsAsync(Guid id)
    {
        using var db = Connection;
        using var multi = await db.QueryMultipleAsync(
            "sp_Example_GetWithDetails",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        var example = await multi.ReadSingleOrDefaultAsync<dbExample>();
        if (example == null) return null;

        var tags = (await multi.ReadAsync<dbTag>()).ToList();
        var history = (await multi.ReadAsync<dbHistoryEntry>()).ToList();

        return new ExampleWithDetails
        {
            Example = example,
            Tags = tags,
            History = history
        };
    }
}
```

**Conventions:**
- One repository per domain entity
- All methods are `async Task<T>`
- Use `using var db = Connection;` to ensure connection disposal
- **All data access goes through stored procedures** — no inline SQL in repositories
- Always pass `commandType: CommandType.StoredProcedure` to Dapper calls
- Use `QueryMultipleAsync` when a stored procedure returns multiple result sets, and assemble the results into composite objects
- Return `db`-prefixed model types from database operations
- Stored procedure naming convention: `sp_{Entity}_{Action}` (e.g., `sp_Example_GetById`, `sp_User_Insert`)

### Database Objects (`DataAccess/Models/DatabaseObjects/`)

Plain C# classes that map directly to database tables. Use the `db` prefix to distinguish them from response DTOs.

```csharp
public class dbExample
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Deleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Conventions:**
- `db` prefix on all database object class names
- Use `required` for non-nullable columns
- Use nullable types (`string?`, `Guid?`) for optional columns
- Include `CreatedAt` and `UpdatedAt` timestamps on all entities
- Use `bool Deleted` for soft-delete support where needed

### Data Transfer Objects (`DataAccess/Models/DataTransferObjects/`)

DTOs define the shape of data entering repositories from controllers. They are distinct from both database objects and response types.

```csharp
public class CreateExampleDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateExampleDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
```

### Response Types (`src/Models/ResponseTypes/`)

Response types define the shape of data returned to API consumers. Use a standard envelope.

```csharp
public class ResponseResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}
```

Domain-specific response classes map from database objects, excluding internal fields and reshaping data for client consumption.

---

## Configuration

### Configuration Helper

A static helper class provides access to `IConfiguration` from anywhere in the application, without dependency injection.

```csharp
// Configuration/ConfigHelper.cs
public static class ConfigHelper
{
    public static IConfiguration Configuration { get; set; } = null!;
}

// Program.cs (before any services are used)
ConfigHelper.Configuration = builder.Configuration;
```

Configuration values are read directly where needed:

```csharp
var apiKey = ConfigHelper.Configuration["MyService:ApiKey"];
var baseUrl = ConfigHelper.Configuration["MyService:BaseUrl"];
```

For services that need config at initialization, pass values explicitly during setup in `Program.cs` rather than reading config inside the service.

### DatabaseManager

A static class that provides the connection string to all repositories. Configures Dapper's snake_case-to-PascalCase column mapping.

```csharp
public static class DatabaseManager
{
    private static IConfiguration? _configuration;

    private static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
            return _configuration;
        }
    }

    public static string ConnectionString =>
        Configuration.GetConnectionString("DefaultConnection")
        ?? throw new MissingFieldException("DefaultConnection not configured");
}
```

### appsettings.json Structure

```json
{
    "Jwt": {
        "Key": "<base64-encoded-signing-key>",
        "Issuer": "MyApi"
    },
    "ConnectionStrings": {
        "DefaultConnection": "Server=...;Database=...;Uid=...;Pwd=...;Pooling=true;Max Pool Size=200;Min Pool Size=5;"
    },
    "MyService": { ... },
    "Logging": {
        "LogLevel": {
            "Default": "Information"
        }
    }
}
```

---

## Authentication & Authorization

### JWT Configuration

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
```

### Token Service

A static helper class that generates JWTs with custom claims. Reads JWT config via `ConfigHelper`. Add whatever domain-specific claims your application requires (e.g., user ID, tenant/org ID, email).

```csharp
public static class TokenService
{
    public static string GenerateToken(Guid userId, string email, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new("user_id", userId.ToString()),
            new(ClaimTypes.Email, email)
        };
        // Add domain-specific claims as needed
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(
            Convert.FromBase64String(ConfigHelper.Configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: ConfigHelper.Configuration["Jwt:Issuer"],
            expires: DateTime.UtcNow.AddMinutes(15),
            claims: claims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Role-Based Authorization

Define roles as string constants in a static class and apply them via attributes.

```csharp
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string User = "User";
    // Add application-specific roles as needed
}

// Usage on endpoints
[Authorize(Roles = UserRoles.Admin)]
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(Guid id) { ... }
```

---

## Middleware Pipeline

Configure middleware in `Program.cs` in the following order:

```csharp
var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();   // 1. Catch unhandled exceptions
app.UseMiddleware<RequestLoggingMiddleware>(); // 2. Log all requests/responses
app.UseCors();                                 // 3. CORS policy
app.UseHttpsRedirection();                     // 4. HTTPS enforcement
app.UseAuthentication();                       // 5. JWT validation
app.UseAuthorization();                        // 6. Role/policy checks
app.MapControllers();                          // 7. Route to controllers
```

### Request Logging Middleware

Captures request/response details to the database for observability:
- HTTP method, path, query string, status code
- Request and response bodies (excluding multipart/form-data)
- User ID extracted from JWT claims
- Processing time in milliseconds
- Request trace ID for correlation

---

## Program.cs Template

```csharp
var builder = WebApplication.CreateBuilder(args);

// --- Static configuration (must come first) ---
ConfigHelper.Configuration = builder.Configuration;

// --- Kestrel configuration ---
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

// --- Authentication ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* see JWT Configuration above */ });
builder.Services.AddAuthorization();

// --- Controllers ---
builder.Services.AddControllers();

// --- Background services (if needed) ---
// builder.Services.AddHostedService<MyBackgroundWorker>();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// --- Forwarded headers (for reverse proxy) ---
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// --- Initialize singleton services ---
MyService.Initialize(
    builder.Configuration["MyService:ApiKey"]!,
    builder.Configuration["MyService:BaseUrl"]!);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Instantiation Patterns

| Component | Pattern | Example |
|---|---|---|
| Repositories | `new` in controllers | `private readonly ExampleRepository _repo = new();` |
| Stateless services | Static class | `TokenService.GenerateToken(...)` |
| Stateful services | Singleton with `Initialize` | `MyService.Instance.DoSomething()` |
| Configuration | Static helper | `ConfigHelper.Configuration["Key"]` |
| Background workers | Hosted Service (only DI used) | `builder.Services.AddHostedService<MyWorker>()` |

The only use of the DI container is for framework-level concerns that require it: authentication, authorization, CORS, hosted background services, and forwarded headers. All application-level code uses static classes or direct instantiation.

---

## Key Technology Choices

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 9.0 |
| ORM | Dapper (micro-ORM, stored procedure calls only) |
| Database | MySQL via MySql.Data |
| Authentication | JWT Bearer tokens |

---

## Patterns & Conventions Summary

1. **Repository pattern** with one repository per domain entity
2. **Stored procedures exclusively** — all query logic lives in the database, repositories are thin wrappers
3. **Snake_case DB columns** mapped automatically to PascalCase C# properties
4. **Standard response envelope** (`ResponseResult<T>`) on all endpoints
5. **Claims-based authorization** with role constants and BaseController helpers
6. **Static configuration helper** for accessing `appsettings.json` anywhere
7. **No dependency injection** for application code — static classes and direct instantiation only
8. **Async/await throughout** — no synchronous database calls
9. **Soft deletes** via `Deleted` boolean flag where needed
10. **Audit timestamps** (`CreatedAt`, `UpdatedAt`) on all entities
