using Slotegrator.Tools.Config;
using Slotegrator.Tools.Constants;
using Slotegrator.Tools.Models;

namespace Slotegrator.Tools.ApiClient;

public class LoginApi
{
    private readonly IApiClient _apiClient;

    public LoginApi(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<HttpResponseMessage> LoginAsync(CredentialsDto credentials)
        => _apiClient.PostAsync(Endpoints.LoginUrl, credentials);

    public async Task<string> GetTokenAsync(UserCredentials userCredentials)
    {
        var credentials = new CredentialsDto
        {
            Email = userCredentials.Email,
            Password = userCredentials.Password
        };

        var loginResponse = await LoginAsync(credentials);
        var token = await loginResponse.ReadRequiredContentAsync<TokenDto>();

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("Login response did not contain an access token.");

        return token.AccessToken;
    }
}
