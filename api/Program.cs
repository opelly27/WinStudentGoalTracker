using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WinStudentGoalTracker.Api.Configuration;
using WinStudentGoalTracker.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();
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
