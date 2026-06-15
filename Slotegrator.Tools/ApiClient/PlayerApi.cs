using System.Collections.Concurrent;
using Slotegrator.Tools.Constants;
using Slotegrator.Tools.Models;

namespace Slotegrator.Tools.ApiClient;

public class PlayerApi
{
    private const int MaxDegreeOfParallelism = 4;

    private readonly IApiClient _apiClient;

    public PlayerApi(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<HttpResponseMessage> CreatePlayerAsync(PlayerRequestDto request, string token)
        => _apiClient.PostWithAuthAsync(Endpoints.CreatePlayerUrl, request, token);

    public Task<HttpResponseMessage> GetPlayerAsync(string email, string token)
        => _apiClient.PostWithAuthAsync(Endpoints.GetPlayerUrl, new PlayerRequestOneDto { Email = email }, token);

    public Task<HttpResponseMessage> GetAllPlayersAsync(string token)
        => _apiClient.GetWithAuthAsync(Endpoints.GetAllPlayersUrl, token);

    public Task<HttpResponseMessage> DeletePlayerAsync(string playerId, string token)
        => _apiClient.DeleteWithAuthAsync(Endpoints.DeletePlayerUrl(playerId), token);

    public async Task<List<PlayerResponseDto>> CreatePlayersAsync(IEnumerable<PlayerRequestDto> players, string token)
    {
        var createdPlayers = new ConcurrentBag<PlayerResponseDto>();

        await Parallel.ForEachAsync(
            players,
            new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
            async (player, cancellationToken) =>
            {
                var response = await CreatePlayerAsync(player, token);
                var created = await response.ReadRequiredContentAsync<PlayerResponseDto>(cancellationToken);
                createdPlayers.Add(created);
            });

        return createdPlayers.ToList();
    }

    public async Task DeletePlayersAsync(IEnumerable<string> playerIds, string token)
    {
        await Parallel.ForEachAsync(
            playerIds,
            new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
            async (playerId, cancellationToken) =>
            {
                var response = await DeletePlayerAsync(playerId, token);
                await response.EnsureSuccessOrThrowAsync(cancellationToken);
            });
    }
}
