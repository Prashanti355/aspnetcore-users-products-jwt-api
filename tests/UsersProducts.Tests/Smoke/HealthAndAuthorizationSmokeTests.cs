using System.Net;

namespace UsersProducts.Tests.Smoke;

public sealed class HealthAndAuthorizationSmokeTests
{
    private readonly ApiTestClient _apiClient = new();

    [Fact]
    public async Task Health_Should_Return_Ok()
    {
        var response = await _apiClient.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Users_Should_Return_Unauthorized_When_Token_Is_Missing()
    {
        var response = await _apiClient.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Products_Public_Catalog_Should_Return_Ok()
    {
        var response = await _apiClient.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Categories_Public_Catalog_Should_Return_Ok()
    {
        var response = await _apiClient.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}