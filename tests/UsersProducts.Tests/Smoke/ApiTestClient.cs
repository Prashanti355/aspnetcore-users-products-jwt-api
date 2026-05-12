using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace UsersProducts.Tests.Smoke;

public sealed class ApiTestClient
{
    private readonly HttpClient _httpClient;

    public ApiTestClient()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? "http://localhost:8080";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public Task<HttpResponseMessage> GetAsync(string path)
    {
        return _httpClient.GetAsync(path);
    }

    public Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string path, TRequest request)
    {
        return _httpClient.PostAsJsonAsync(path, request);
    }

    public Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(
        string path,
        TRequest request,
        string token)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(request)
        };

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return _httpClient.SendAsync(message);
    }

    public Task<HttpResponseMessage> GetAsync(string path, string token)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, path);

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return _httpClient.SendAsync(message);
    }
}