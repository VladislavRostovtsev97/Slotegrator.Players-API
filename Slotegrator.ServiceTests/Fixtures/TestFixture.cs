using System.Net.Http.Json;
using NUnit.Framework;
using Slotegrator.Tools.ApiClient;
using Slotegrator.Tools.Config;
using Slotegrator.Tools.Models;

namespace Slotegrator.ServiceTests.Fixtures;

public abstract class TestFixture
{
    protected SlotegratorApiClient ApiClient = null!;
    protected LoginApi LoginApi = null!;
    protected PlayerApi PlayerApi = null!;
    protected AppConfig Config = null!;
    protected string AuthToken = null!;

    [OneTimeSetUp]
    public void SetupResourcesBeforeAllTests()
    {
        Config = ConfigLoader.Load();
        ApiClient = new SlotegratorApiClient(Config.BaseUrl, log: message => TestContext.Out.WriteLine(message));
        LoginApi = new LoginApi(ApiClient);
        PlayerApi = new PlayerApi(ApiClient);
    }

    protected Task<string> GetTokenAsync() => LoginApi.GetTokenAsync(Config.UserCredentials);
    
    protected async Task DeleteAllPlayersAsync(string token)
    {
        var response = await PlayerApi.GetAllPlayersAsync(token);
        var players = await response.Content.ReadFromJsonAsync<IReadOnlyList<PlayerResponseDto>>() ?? [];

        var playerIds = players
            .Select(player => player.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        if (playerIds.Count > 0)
            await PlayerApi.DeletePlayersAsync(playerIds, token);
    }

    [OneTimeTearDown]
    public void DisposeResourcesAfterAllTests() => ApiClient.Dispose();
}
