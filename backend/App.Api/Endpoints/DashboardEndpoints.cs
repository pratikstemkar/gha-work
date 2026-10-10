using System.Security.Claims;
using App.Api.Models;
using App.Api.Services;

namespace App.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard", (ClaimsPrincipal principal, ItemStore items) =>
        {
            var all = items.All();
            var completed = all.Count(i => i.Done);
            var user = UserDto.FromPrincipal(principal);

            return Results.Ok(new DashboardResponse(
                $"Welcome back, {user.DisplayName}!",
                user,
                all.Count,
                completed,
                all.Count - completed));
        })
        .WithTags("Dashboard")
        .RequireAuthorization();

        return app;
    }
}
