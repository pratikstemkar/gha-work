using System.Security.Claims;

namespace App.Api.Models;

/// <summary>A user account kept in memory (no database at this stage).</summary>
public record UserAccount(
    string Username,
    string Password,
    string DisplayName,
    string Email,
    string[] Roles);

/// <summary>Public shape of a user returned to the client (never includes the password).</summary>
public record UserDto(string Username, string DisplayName, string Email, string[] Roles)
{
    public static UserDto From(UserAccount user) =>
        new(user.Username, user.DisplayName, user.Email, user.Roles);

    public static UserDto FromPrincipal(ClaimsPrincipal principal) =>
        new(
            principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            principal.FindFirstValue("displayName") ?? string.Empty,
            principal.FindFirstValue("email") ?? string.Empty,
            principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray());
}

public record LoginRequest(string? Username, string? Password);

public record LoginResponse(string Token, UserDto User);

public record ItemDto(int Id, string Title, bool Done, DateTime CreatedAt)
{
    public static ItemDto From(Services.ItemStore.Item item) =>
        new(item.Id, item.Title, item.Done, item.CreatedAt);
}

public record CreateItemRequest(string? Title);

public record UpdateItemRequest(bool Done);

public record DashboardResponse(
    string Greeting,
    UserDto User,
    int TotalItems,
    int CompletedItems,
    int PendingItems);
