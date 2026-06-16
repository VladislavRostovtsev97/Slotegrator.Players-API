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

public class RegisterPlayerTests : TestFixture
{
    [SetUp]
    public async Task AuthenticateAndResetState()
    {
        AuthToken = await GetTokenAsync();
        await DeleteAllPlayersAsync(AuthToken);
    }

    [Test]
    [Property("TestId", "RegisterPlayerTests_01")]
    public async Task RegisterPlayerTests_RegisterSingle_ReturnsCreatedPlayer()
    {
        //Arrange
        var expectedPlayer = PlayerFactory.CreatePlayer();

        //Act
        var createPlayerResponse = await PlayerApi.CreatePlayerAsync(expectedPlayer, AuthToken);
        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponseDto>();

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(createPlayerResponse, HttpStatusCode.Created);
        using (new AssertionScope())
        {
            createdPlayer.Should().NotBeNull();
            createdPlayer!.Id.Should().NotBeNullOrWhiteSpace();
            createdPlayer.Email.Should().Be(expectedPlayer.Email);
            createdPlayer.Name.Should().Be(expectedPlayer.Name);
            createdPlayer.Surname.Should().Be(expectedPlayer.Surname);
            createdPlayer.Username.Should().Be(expectedPlayer.Username);
            createdPlayer.CurrencyCode.Should().Be(expectedPlayer.CurrencyCode);
        }
    }

    [TestCase(TestData.PlayerCount)]
    [Property("TestId", "RegisterPlayerTests_02")]
    public async Task RegisterPlayerTests_CreateMultiplePlayers_AllCreated(int playerCount)
    {
        //Arrange
        var expectedPlayers = PlayerFactory.CreatePlayers(playerCount);

        //Act
        var createdPlayers = await PlayerApi.CreatePlayersAsync(expectedPlayers, AuthToken);

        //Assert
        using (new AssertionScope())
        {
            createdPlayers.Should().HaveCount(expectedPlayers.Count);
            createdPlayers.Should().OnlyContain(player => !string.IsNullOrWhiteSpace(player.Id));

            createdPlayers
                .Select(player => new
                {
                    player.Email, 
                    player.Username, 
                    player.Name, 
                    player.Surname, 
                    player.CurrencyCode
                })
                .Should().BeEquivalentTo(expectedPlayers
                    .Select(player => new
                    {
                        player.Email, 
                        player.Username, 
                        player.Name, 
                        player.Surname, 
                        player.CurrencyCode
                    }));
        }
    }
}
