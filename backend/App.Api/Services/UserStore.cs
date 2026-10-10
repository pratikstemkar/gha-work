using App.Api.Models;

namespace App.Api.Services;

/// <summary>
/// In-memory user directory. Replace with a real database and password hashing
/// before this is ever used for anything real.
/// </summary>
public class UserStore
{
    private readonly List<UserAccount> _users =
    [
        new("demo", "demo123", "Demo User", "demo@example.com", ["User"]),
        new("admin", "admin123", "Admin User", "admin@example.com", ["Admin"]),
    ];

    public UserAccount? Validate(string username, string password) =>
        _users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

    public UserAccount? Find(string username) =>
        _users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
}
