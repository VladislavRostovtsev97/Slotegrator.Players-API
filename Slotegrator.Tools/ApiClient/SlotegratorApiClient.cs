using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Slotegrator.Tools.ApiClient;

public class SlotegratorApiClient : IApiClient, IDisposable
{
    private readonly HttpClient _httpClient;

    public SlotegratorApiClient(string baseUrl, Action<string>? log = null)
    {
        _httpClient = new HttpClient(new LoggingHandler(log ?? Console.WriteLine))
        {
            BaseAddress = new Uri(baseUrl)
        };
    }
    

    public Task<HttpResponseMessage> PostAsync(string url, object? content = null)
        => SendAsync(HttpMethod.Post, url, content);

    public Task<HttpResponseMessage> PostWithAuthAsync(string url, object? content, string token)
        => SendAsync(HttpMethod.Post, url, content, BearerHeader(token));

    public Task<HttpResponseMessage> GetWithAuthAsync(string url, string token)
        => SendAsync(HttpMethod.Get, url, auth: BearerHeader(token));

    public Task<HttpResponseMessage> DeleteWithAuthAsync(string url, string token)
        => SendAsync(HttpMethod.Delete, url, auth: BearerHeader(token));

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        object? content = null,
        AuthenticationHeaderValue? auth = null)
    {
        using var request = new HttpRequestMessage(method, url);

        if (auth != null)
            request.Headers.Authorization = auth;

        if (content != null)
            request.Content = JsonContent.Create(content, content.GetType());

        return await _httpClient.SendAsync(request);
    }

    private static AuthenticationHeaderValue BearerHeader(string token)
        => new("Bearer", token);

    public void Dispose() => _httpClient.Dispose();
}
