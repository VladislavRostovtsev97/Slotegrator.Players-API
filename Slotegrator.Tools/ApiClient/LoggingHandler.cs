using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Slotegrator.Tools.ApiClient;

public class LoggingHandler : DelegatingHandler
{
    private static readonly Regex SensitiveJsonField = new(
        "\"(?<key>password|password_change|password_repeat|accessToken|access_token|token)\"\\s*:\\s*\"[^\"]*\"",
        RegexOptions.IgnoreCase);

    private readonly Action<string> _log;

    public LoggingHandler(Action<string> log, HttpMessageHandler? innerHandler = null)
    {
        _log = log;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        var responseBody = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine($"-> {request.Method} {request.RequestUri}");
        if (!string.IsNullOrWhiteSpace(requestBody))
            builder.AppendLine($"   request : {MaskSensitiveData(requestBody)}");
        builder.AppendLine($"<- {(int)response.StatusCode} {response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)");
        if (!string.IsNullOrWhiteSpace(responseBody))
            builder.AppendLine($"   response: {MaskSensitiveData(responseBody)}");

        _log(builder.ToString().TrimEnd());

        return response;
    }

    // Masks sensitive values so credentials/tokens never leak into test output Logs.
    private static string MaskSensitiveData(string body)
        => SensitiveJsonField.Replace(body, match => $"\"{match.Groups["key"].Value}\":\"***\"");
}
