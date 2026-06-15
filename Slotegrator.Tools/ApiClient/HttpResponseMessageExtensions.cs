using System.Net.Http.Json;

namespace Slotegrator.Tools.ApiClient;

public static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessOrThrowAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException(
            $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} failed: " +
            $"{(int)response.StatusCode} {response.StatusCode}. Body: {body}");
    }
    
    public static async Task<T> ReadRequiredContentAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        await response.EnsureSuccessOrThrowAsync(cancellationToken);

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
               ?? throw new InvalidOperationException(
                   $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} returned an empty body.");
    }
}
