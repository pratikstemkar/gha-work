using System.Text;
using App.Api.Endpoints;
using App.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI document (served at /openapi/v1.json in development).
builder.Services.AddOpenApi();

// In-memory stores and auth helpers. Swap these for a database later.
builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<ItemStore>();
builder.Services.AddSingleton<TokenService>();

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep claim types exactly as issued (e.g. "email") instead of letting
        // the handler rewrite them to long WS-* URIs.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        };
    });

builder.Services.AddAuthorization();

// The Vite dev server is a separate origin, so allow it during development.
const string SpaCors = "spa";
builder.Services.AddCors(options => options.AddPolicy(SpaCors, policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(SpaCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" })).WithTags("Health");

app.MapAuthEndpoints();
app.MapDashboardEndpoints();
app.MapItemEndpoints();

app.Run();

// Exposed so integration tests can reference the entry point (WebApplicationFactory).
public partial class Program;
