using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Slotegrator.ServiceTests.Fixtures;
using Slotegrator.ServiceTests.Helpers;
using Slotegrator.Tools.Builders;
using Slotegrator.Tools.Constants;
using Slotegrator.Tools.Models;

namespace Slotegrator.ServiceTests.Tests;

public class DeletePlayersTests : TestFixture
{
    [SetUp]
    public async Task AuthenticateAndResetState()
    {
        AuthToken = await GetTokenAsync();
        await DeleteAllPlayersAsync(AuthToken);
    }

    [Test]
    [Property("TestId", "DeletePlayersTests_01")]
    public async Task DeletePlayersTests_ExistingPlayer_RemovesPlayer()
    {
        //Arrange
        var player = PlayerFactory.CreatePlayer();
        var createResponse = await PlayerApi.CreatePlayerAsync(player, AuthToken);
        var createdPlayer = await createResponse.Content.ReadFromJsonAsync<PlayerResponseDto>();
        createdPlayer.Should().NotBeNull();

        //Act
        var deleteResponse = await PlayerApi.DeletePlayerAsync(createdPlayer!.Id, AuthToken);

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(deleteResponse, HttpStatusCode.OK);
        
        var getPlayerResponse = await PlayerApi.GetPlayerAsync(player.Email, AuthToken);
        await AssertionHelper.EnsureStatusCodeAsync(getPlayerResponse, HttpStatusCode.BadRequest);
    }

    [Test]
    [Property("TestId", "DeletePlayersTests_02")]
    public async Task DeletePlayersTests_RemovesAll_GetAllReturnsEmpty()
    {
        //Arrange
        var expectedPlayers = PlayerFactory.CreatePlayers(TestData.PlayerCount);
        var createdPlayers = await PlayerApi.CreatePlayersAsync(expectedPlayers, AuthToken);
        var createdPlayersIds = createdPlayers.Select(p => p.Id).ToList();

        //Act
        await PlayerApi.DeletePlayersAsync(createdPlayersIds, AuthToken);

        //Assert
        var getAllPlayersResponse = await PlayerApi.GetAllPlayersAsync(AuthToken);
        var allPlayers = await getAllPlayersResponse.Content.ReadFromJsonAsync<IReadOnlyList<PlayerResponseDto>>();

        await AssertionHelper.EnsureStatusCodeAsync(getAllPlayersResponse, HttpStatusCode.OK);
        using (new AssertionScope())
        {
            allPlayers.Should().NotBeNull();
            allPlayers.Should().BeEmpty();
        }
    }
}
