using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Slotegrator.ServiceTests.Fixtures;
using Slotegrator.ServiceTests.Helpers;
using Slotegrator.Tools.Builders;
using Slotegrator.Tools.Models;

namespace Slotegrator.ServiceTests.Tests;

public class GetPlayerDataTests : TestFixture
{
    [SetUp]
    public async Task AuthenticateAndResetState()
    {
        AuthToken = await GetTokenAsync();
        await DeleteAllPlayersAsync(AuthToken);
    }

    [Test]
    [Property("TestId", "GetPlayerDataTests_01")]
    public async Task GetPlayerDataTests_GetExistingPlayer_ReturnsMatchingData()
    {
        //Arrange
        var expectedPlayer = PlayerFactory.CreatePlayer();
        var createPlayerResponse = await PlayerApi.CreatePlayerAsync(expectedPlayer, AuthToken);
        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponseDto>();
        createdPlayer.Should().NotBeNull();

        //Act
        var getPlayerResponse = await PlayerApi.GetPlayerAsync(expectedPlayer.Email, AuthToken);
        var getPlayerBody = await getPlayerResponse.Content.ReadFromJsonAsync<PlayerResponseDto>();

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(getPlayerResponse, HttpStatusCode.Created);
        using (new AssertionScope())
        {
            getPlayerBody.Should().NotBeNull();
            getPlayerBody!.Id.Should().Be(createdPlayer!.Id);
            getPlayerBody.Email.Should().Be(expectedPlayer.Email);
            getPlayerBody.Name.Should().Be(expectedPlayer.Name);
            getPlayerBody.Surname.Should().Be(expectedPlayer.Surname);
            getPlayerBody.Username.Should().Be(expectedPlayer.Username);
            getPlayerBody.CurrencyCode.Should().Be(expectedPlayer.CurrencyCode);
        }
    }

    [Test]
    [Property("TestId", "GetPlayerDataTests_02")]
    public async Task GetPlayerDataTests_GetNonExistentPlayer_Returns400()
    {
        //Arrange
        var nonExistentEmail = $"missing_{Guid.NewGuid():N}@example.com";

        //Act
        var getPlayerResponse = await PlayerApi.GetPlayerAsync(nonExistentEmail, AuthToken);

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(getPlayerResponse, HttpStatusCode.BadRequest);
    }

    [Test]
    [Property("TestId", "GetPlayerDataTests_03")]
    public async Task GetPlayerDataTests_GetAllPlayers_Returns200AndCheckSortingByName()
    {
        //Arrange
        var expectedSortedPlayerNames = new[] { "Alex", "Bob", "Celin", "Daniel", "George" };
        var playersToCreate = expectedSortedPlayerNames
            .Select(name => new PlayerBuilder().WithName(name).Build())
            .ToList();
        await PlayerApi.CreatePlayersAsync(playersToCreate, AuthToken);

        //Act
        var getAllPlayersResponse = await PlayerApi.GetAllPlayersAsync(AuthToken);
        var allPlayers = await getAllPlayersResponse.Content.ReadFromJsonAsync<IReadOnlyList<PlayerResponseDto>>();
        
        
        var sortedNames = allPlayers!
            .Select(player => player.Name)
            .OrderBy(name => name)
            .ToList();

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(getAllPlayersResponse, HttpStatusCode.OK);
        using (new AssertionScope())
        {
            allPlayers.Should().NotBeNull();
            sortedNames.Should().Equal(expectedSortedPlayerNames);
        }
    }
}
