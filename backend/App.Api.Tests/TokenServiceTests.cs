using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Api.Models;
using App.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.Api.Tests;

public class TokenServiceTests
{
    private const string Key = "test-signing-key-that-is-long-enough-for-hs256-0123456789";

    private static TokenService CreateService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "App.Api.Tests",
                ["Jwt:Audience"] = "App.Client.Tests",
                ["Jwt:Key"] = Key,
            })
            .Build();

        return new TokenService(configuration);
    }

    private static readonly UserAccount DemoUser =
        new("demo", "demo123", "Demo User", "demo@example.com", ["User", "Admin"]);

    [Fact]
    public void CreateToken_ProducesTokenWithExpectedClaims()
    {
        var token = CreateService().CreateToken(DemoUser);

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("App.Api.Tests", parsed.Issuer);
        Assert.Contains(parsed.Claims, c => c.Type == ClaimTypes.Name && c.Value == "demo");
        Assert.Contains(parsed.Claims, c => c.Type == "displayName" && c.Value == "Demo User");
        Assert.Contains(parsed.Claims, c => c.Type == "email" && c.Value == "demo@example.com");
        Assert.Equal(2, parsed.Claims.Count(c => c.Type == ClaimTypes.Role));
    }

    [Fact]
    public void CreateToken_IsValidatedByMatchingParameters()
    {
        var token = CreateService().CreateToken(DemoUser);

        var principal = new JwtSecurityTokenHandler().ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "App.Api.Tests",
                ValidAudience = "App.Client.Tests",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            },
            out _);

        Assert.Equal("demo", principal.FindFirstValue(ClaimTypes.Name));
    }
}
