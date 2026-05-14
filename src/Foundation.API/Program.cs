using Foundation.API.Endpoints;
using Foundation.Application.Extensions;
using Foundation.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Foundation.API.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Observability
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["Observability:ApplicationInsights:ConnectionString"];
});

// Database & services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Authentication & Authorization
// Custom JWT bearer tokens validated via the application's shared secret (FR-1, NFR-3, NFR-6).
var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
var signingKey = jwtSettings["SigningKey"]
    ?? throw new InvalidOperationException("JWT signing key is not configured. Set 'Authentication:Jwt:SigningKey'.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
    options.AddPolicy("LeadershipPolicy", policy => policy.RequireRole("Leadership"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://app.corp.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Rate limiting (built-in .NET 8 rate limiting middleware)
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("Fixed", _ =>
        RateLimitPartition.GetTokenBucketLimiter("Global", key => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 100,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 100,
            AutoReplenishment = true
        }));
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Prometheus metrics collection
app.UseHttpMetrics();

app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", ([FromServices] Foundation.Infrastructure.Data.FoundationDbContext context) =>
{
    var canConnect = context.Database.CanConnect();
    return canConnect ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503);
});

app.MapHealthChecks("/health");

app.MapMetrics("/metrics");

app.MapGet("/employees", [Authorize(Policy = "EmployeePolicy")] ([FromServices] Foundation.Application.Services.IEmployeeService service) =>
    Results.Ok(service.ListAsync()));

app.MapGet("/employees/{id}", [Authorize(Policy = "LeadershipPolicy")] ([FromServices] Foundation.Application.Services.IEmployeeService service, Guid id) =>
    service.GetAsync(id) switch
    {
        { } employee => Results.Ok(employee),
        null => Results.NotFound()
    });

app.MapPost("/employees", [Authorize(Policy = "AdminPolicy")] ([FromServices] Foundation.Application.Services.IEmployeeService service, [FromBody] Foundation.Domain.Entities.Employee employee) =>
{
    var task = service.GetAsync(employee.Id);
    return Results.Accepted();
});

// Profile endpoints (FR-2, FR-3, FR-4, FR-5, FR-6)
app.MapProfileEndpoints();

app.Run();
