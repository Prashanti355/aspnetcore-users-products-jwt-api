using System.Net;
using System.Net.Http.Json;

namespace UsersProducts.Tests.Smoke;

public sealed class AdminAccessSmokeTests
{
    private readonly ApiTestClient _apiClient = new();

    [Fact]
    public async Task Users_Admin_Endpoint_Should_Return_Ok_With_Admin_Token()
    {
        var token = await LoginAsAdminAsync();

        var response = await _apiClient.GetAsync("/api/users", token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Products_Admin_Endpoint_Should_Return_Ok_With_Admin_Token()
    {
        var token = await LoginAsAdminAsync();

        var response = await _apiClient.GetAsync("/api/products/admin", token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Categories_Admin_Endpoint_Should_Return_Ok_With_Admin_Token()
    {
        var token = await LoginAsAdminAsync();

        var response = await _apiClient.GetAsync("/api/categories/admin", token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> LoginAsAdminAsync()
    {
        var request = new LoginRequest(
            Email: Environment.GetEnvironmentVariable("ADMIN_SEED_EMAIL")
                ?? "admin.seed@example.com",
            Password: Environment.GetEnvironmentVariable("ADMIN_SEED_PASSWORD")
                ?? "Admin1234"
        );

        var response = await _apiClient.PostAsJsonAsync("/api/auth/login", request);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));

        return body.AccessToken;
    }

    private sealed record LoginRequest(
        string Email,
        string Password
    );

    private sealed record AuthResponse(
        string AccessToken,
        DateTime ExpiresAtUtc,
        UserResponse User
    );

    private sealed record UserResponse(
        Guid Id,
        string Name,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc
    );
}