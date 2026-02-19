// =============================================================================
// Program.cs
// =============================================================================
// This is the application's entry point — where everything gets wired together.
//
// It does three things:
//   1. REGISTERS SERVICES with the dependency injection (DI) container
//   2. CONFIGURES the HTTP pipeline (middleware order matters!)
//   3. STARTS the web server
//
// DEPENDENCY INJECTION (DI) IN PLAIN ENGLISH:
// -------------------------------------------
// Instead of creating objects yourself (e.g., `new DummyGoalRepository()`),
// you tell the framework: "whenever something asks for IGoalRepository,
// give it a DummyGoalRepository." Then your controllers just declare
// "I need an IGoalRepository" in their constructor, and the framework
// automatically provides one.
//
// This is powerful because:
//   - You can swap implementations in ONE place (here) without touching
//     any controllers or handlers.
//   - Each class only knows about the INTERFACE, not the concrete class.
//   - Testing is easier — you can provide fake implementations.
//
// SERVICE LIFETIMES:
//   - AddSingleton: ONE instance for the entire application lifetime.
//   - AddScoped:    ONE instance PER HTTP REQUEST (created at the start
//                   of the request, disposed at the end).
//   - AddTransient: A NEW instance every time it's requested.
//
// We use AddSingleton for repositories because our dummy data is static
// (shared across all requests). In production with a real database, you'd
// use AddScoped so each request gets its own database connection.
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using RolesAssignments.Authorization.Handlers;
using RolesAssignments.Data;
using RolesAssignments.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// SERVICE REGISTRATION
// =============================================================================

// ---- Controllers ----
// Tells the framework to discover and register all [ApiController] classes.
builder.Services.AddControllers();

// ---- Authentication (Fake) ----
// ASP.NET Core's [Authorize] attribute needs a registered authentication scheme
// so it knows HOW to reject unauthenticated requests (the "challenge").
// Even though our FakeAuthMiddleware does the real work of setting up the user,
// we still need to register a scheme name so the framework doesn't complain.
//
// In production, you'd replace this with:
//   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//       .AddJwtBearer(options => { /* JWT config */ });
builder.Services.AddAuthentication("FakeAuth")
    .AddCookie("FakeAuth", options =>
    {
        // These settings don't really matter for our prototype since
        // FakeAuthMiddleware handles everything. But the framework requires
        // a registered handler for the scheme to exist.
        options.Events.OnRedirectToLogin = context =>
        {
            // Instead of redirecting to a login page (default cookie behavior),
            // return a clean 401 status code — appropriate for an API.
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

// ---- Authorization ----
// Registers the core authorization services (IAuthorizationService, etc.).
// This is what makes AuthorizeAsync() available for injection.
builder.Services.AddAuthorizationCore();

// ---- Authorization Handlers ----
// Register our custom authorization handlers. The framework will discover
// them when AuthorizeAsync() is called and route to the correct handler
// based on the requirement type and resource type.
//
// AddScoped means one instance per request — this is appropriate because
// each handler may hold per-request state (though ours don't).
builder.Services.AddScoped<IAuthorizationHandler, StudentAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProgressEntryAuthorizationHandler>();

// ---- Repositories (Data Access) ----
// Here's where the "swap" happens. Today we register dummy implementations.
// To switch to a real database, you'd change these lines to:
//
//   builder.Services.AddScoped<IAssignmentRepository, DapperAssignmentRepository>();
//   builder.Services.AddScoped<IStudentRepository, DapperStudentRepository>();
//   builder.Services.AddScoped<IGoalRepository, DapperGoalRepository>();
//   builder.Services.AddScoped<IProgressEntryRepository, DapperProgressEntryRepository>();
//
// ...and nothing else in the entire codebase would change.
builder.Services.AddSingleton<IAssignmentRepository, DummyAssignmentRepository>();
builder.Services.AddSingleton<IStudentRepository, DummyStudentRepository>();
builder.Services.AddSingleton<IGoalRepository, DummyGoalRepository>();
builder.Services.AddSingleton<IProgressEntryRepository, DummyProgressEntryRepository>();

// ---- OpenAPI / Swagger UI ----
// AddEndpointsApiExplorer lets Swagger discover your endpoints.
// AddSwaggerGen generates the OpenAPI spec AND the interactive test page.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // This filter adds the "X-User-Id" header input to every endpoint in the
    // Swagger UI, so you don't have to type the header name manually each time.
    options.OperationFilter<FakeAuthHeaderFilter>();
});

var app = builder.Build();

// =============================================================================
// MIDDLEWARE PIPELINE
// =============================================================================
// Middleware runs in ORDER. Each request flows through them like this:
//
//   Request
//     → HTTPS Redirection
//     → FakeAuthMiddleware (sets up the user)
//     → Authorization (checks [Authorize] attributes)
//     → Controller (your endpoint code)
//   Response
//
// ORDER MATTERS! FakeAuthMiddleware must run BEFORE UseAuthorization(),
// because authorization needs to know who the user is before it can decide
// if they're allowed in. If you swap them, every request would be denied
// because the user would still be "anonymous" when authorization runs.

if (app.Environment.IsDevelopment())
{
    // Serves the OpenAPI JSON spec at /swagger/v1/swagger.json
    app.UseSwagger();

    // Serves the interactive Swagger UI at /swagger
    // To test as different users, click "Try it out" on any endpoint,
    // then add the header: X-User-Id with value 1, 2, or 3.
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the authentication middleware (needed to activate the "FakeAuth" scheme).
app.UseAuthentication();

// Our fake authentication middleware runs AFTER UseAuthentication() and OVERRIDES
// the user identity based on the X-User-Id header. In production, you'd remove
// this line entirely — UseAuthentication() above would handle everything via JWT.
app.UseMiddleware<FakeAuthMiddleware>();

// ASP.NET Core's built-in authorization middleware. This is what enforces
// [Authorize] attributes on controllers and calls our handlers.
app.UseAuthorization();

// Maps controller routes (e.g., /api/students, /api/students/101/goals)
app.MapControllers();

app.Run();
