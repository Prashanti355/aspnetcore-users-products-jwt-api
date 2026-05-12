using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace UsersProducts.Tests.Smoke;

public sealed class AuthSmokeTests
{
    private readonly ApiTestClient _apiClient = new();

    [Fact]
    public async Task Admin_Login_Should_Return_Access_Token()
    {
        var request = new LoginRequest(
            Email: Environment.GetEnvironmentVariable("ADMIN_SEED_EMAIL")
                ?? "admin.seed@example.com",
            Password: Environment.GetEnvironmentVariable("ADMIN_SEED_PASSWORD")
                ?? "Admin1234"
        );

        var response = await _apiClient.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.NotNull(body.User);
        Assert.Equal("Admin", body.User.Role);
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