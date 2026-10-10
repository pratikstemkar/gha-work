using App.Api.Services;

namespace App.Api.Tests;

public class UserStoreTests
{
    private readonly UserStore _store = new();

    [Fact]
    public void Validate_ReturnsUser_ForCorrectCredentials()
    {
        var user = _store.Validate("demo", "demo123");

        Assert.NotNull(user);
        Assert.Equal("demo", user!.Username);
        Assert.Equal("Demo User", user.DisplayName);
    }

    [Theory]
    [InlineData("demo", "wrong")]
    [InlineData("nobody", "demo123")]
    [InlineData("", "")]
    public void Validate_ReturnsNull_ForBadCredentials(string username, string password)
    {
        Assert.Null(_store.Validate(username, password));
    }

    [Fact]
    public void Validate_IsCaseInsensitive_ForUsername()
    {
        var user = _store.Validate("DEMO", "demo123");

        Assert.NotNull(user);
        Assert.Equal("demo", user!.Username);
    }

    [Fact]
    public void Validate_IsCaseSensitive_ForPassword()
    {
        Assert.Null(_store.Validate("demo", "DEMO123"));
    }

    [Fact]
    public void Find_ReturnsUser_RegardlessOfCase()
    {
        Assert.NotNull(_store.Find("ADMIN"));
    }
}
