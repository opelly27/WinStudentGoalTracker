using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WinStudentGoalTracker.Api.Configuration;
using WinStudentGoalTracker.Services;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Build connection string from .env variables
var dbServer = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3309";
var dbName = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "winstudentgoaltracker";
var dbUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
var dbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "";
builder.Configuration["ConnectionStrings:DefaultConnection"] =
    $"Server={dbServer};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};";

// Override JWT key from .env if present
var envJwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (!string.IsNullOrEmpty(envJwtKey))
    builder.Configuration["Jwt:Key"] = envJwtKey;

Console.WriteLine($"Built connection string from .env: {builder.Configuration["ConnectionStrings:DefaultConnection"]}");

ConfigHelper.Configuration = builder.Configuration;

var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_change_me_in_production_123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WinStudentGoalTrackerAPI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<TokenService>();

builder.Services.AddHttpClient<TranscriptionService>(client =>
{
    client.BaseAddress = new Uri("https://stt.opelly.me");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Ollama:BaseUrl"] ?? "https://llm.opelly.me");
    client.Timeout = TimeSpan.FromMinutes(3);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer ' prefix)."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
