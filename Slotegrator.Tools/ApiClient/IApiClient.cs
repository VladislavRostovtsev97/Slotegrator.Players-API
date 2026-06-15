namespace Slotegrator.Tools.ApiClient;

public interface IApiClient
{
    Task<HttpResponseMessage> PostAsync(string url, object? content = null);

    Task<HttpResponseMessage> PostWithAuthAsync(string url, object? content, string token);

    Task<HttpResponseMessage> GetWithAuthAsync(string url, string token);

    Task<HttpResponseMessage> DeleteWithAuthAsync(string url, string token);
}
