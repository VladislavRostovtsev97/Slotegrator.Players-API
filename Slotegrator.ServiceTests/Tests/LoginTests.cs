using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.IdentityModel.JsonWebTokens;
using NUnit.Framework;
using Slotegrator.ServiceTests.Fixtures;
using Slotegrator.ServiceTests.Helpers;
using Slotegrator.Tools.Constants;
using Slotegrator.Tools.Models;

namespace Slotegrator.ServiceTests.Tests;

public class LoginTests : TestFixture
{
    [Test]
    [Property("TestId", "LoginTests_01")]
    public async Task LoginTests_WithValidCredentials_Returns201AndToken()
    {
        //Arrange
        var credentials = new CredentialsDto
        {
            Email = Config.UserCredentials.Email,
            Password = Config.UserCredentials.Password
        };

        //Act
        var loginResponse = await LoginApi.LoginAsync(credentials);
        var token = await loginResponse.Content.ReadFromJsonAsync<TokenDto>();

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(loginResponse, HttpStatusCode.Created);
        using (new AssertionScope())
        {
            token.Should().NotBeNull();
            token!.AccessToken.Should().NotBeNullOrWhiteSpace();
            
            // Decode the JWT payload and check the "email" claim actually belong to the user we logged in.
            var emailClaim = new JsonWebToken(token.AccessToken).GetClaim("email").Value;
            emailClaim.Should().Be(Config.UserCredentials.Email);
        }
    }

    [Test]
    [Property("TestId", "LoginTests_02")]
    public async Task LoginTests_WithInvalidCredentials_Returns401()
    {
        //Arrange
        var credentials = new CredentialsDto
        {
            Email = Config.UserCredentials.Email,
            Password = TestData.InvalidPassword
        };

        //Act
        var response = await LoginApi.LoginAsync(credentials);

        //Assert
        await AssertionHelper.EnsureStatusCodeAsync(response, HttpStatusCode.Unauthorized);
    }
}
