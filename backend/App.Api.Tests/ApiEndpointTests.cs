using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using App.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace App.Api.Tests;

/// <summary>
/// End-to-end tests against the real HTTP pipeline, including JWT auth.
/// </summary>
public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Do not run development hosting startup assemblies (e.g. the SPA proxy).
            builder.UseEnvironment("Testing");
            builder.UseSetting("hostingStartupAssemblies", string.Empty);
        });
    }

    private static async Task<LoginResponse> LoginAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "demo", password = "demo123" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var login = await LoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
        return client;
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUser()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "demo", password = "demo123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal("demo", body.User.Username);
        Assert.Contains("User", body.User.Roles);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401WithMessage()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "demo", password = "nope" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.Equal("Invalid username or password.", payload!["message"]);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Dashboard_WithToken_ReturnsCountsAndUser()
    {
        var client = await CreateAuthenticatedClientAsync(_factory);

        var response = await client.GetAsync("/api/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadFromJsonAsync<DashboardResponse>())!;
        Assert.Equal("demo", body.User.Username);
        Assert.Equal(body.TotalItems, body.CompletedItems + body.PendingItems);
    }

    [Fact]
    public async Task Me_WithToken_ReturnsIdentityIncludingEmail()
    {
        var client = await CreateAuthenticatedClientAsync(_factory);

        var body = await client.GetFromJsonAsync<UserDto>("/api/auth/me");

        Assert.NotNull(body);
        Assert.Equal("demo", body!.Username);
        Assert.Equal("demo@example.com", body.Email);
    }

    [Fact]
    public async Task Items_FullLifecycle_Works()
    {
        var client = await CreateAuthenticatedClientAsync(_factory);

        // Create.
        var created = await client.PostAsJsonAsync("/api/items", new { title = "Integration item" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var item = (await created.Content.ReadFromJsonAsync<ItemDto>())!;
        Assert.Equal("Integration item", item.Title);
        Assert.False(item.Done);

        // It shows up in the list.
        var list = (await client.GetFromJsonAsync<List<ItemDto>>("/api/items"))!;
        Assert.Contains(list, i => i.Id == item.Id);

        // Toggle it done.
        var patched = await client.PatchAsJsonAsync($"/api/items/{item.Id}", new { done = true });
        Assert.Equal(HttpStatusCode.OK, patched.StatusCode);
        var updated = (await patched.Content.ReadFromJsonAsync<ItemDto>())!;
        Assert.True(updated.Done);

        // Delete it.
        var deleted = await client.DeleteAsync($"/api/items/{item.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var after = (await client.GetFromJsonAsync<List<ItemDto>>("/api/items"))!;
        Assert.DoesNotContain(after, i => i.Id == item.Id);
    }

    [Fact]
    public async Task CreateItem_WithBlankTitle_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/items", new { title = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_WithUnknownId_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync(_factory);

        var response = await client.PatchAsJsonAsync("/api/items/999999", new { done = true });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
