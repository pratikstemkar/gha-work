using System.Security.Claims;
using App.Api.Models;
using App.Api.Services;

namespace App.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", (LoginRequest request, UserStore users, TokenService tokens) =>
        {
            var user = users.Validate(request.Username ?? string.Empty, request.Password ?? string.Empty);
            if (user is null)
            {
                return Results.Json(new { message = "Invalid username or password." }, statusCode: StatusCodes.Status401Unauthorized);
            }

            return Results.Ok(new LoginResponse(tokens.CreateToken(user), UserDto.From(user)));
        })
        .AllowAnonymous();

        group.MapGet("/me", (ClaimsPrincipal principal) => Results.Ok(UserDto.FromPrincipal(principal)))
            .RequireAuthorization();

        return app;
    }
}
