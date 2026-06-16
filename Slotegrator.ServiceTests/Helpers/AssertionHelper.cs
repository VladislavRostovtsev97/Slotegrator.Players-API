using System.Net;
using FluentAssertions;

namespace Slotegrator.ServiceTests.Helpers;

public static class AssertionHelper
{
    public static async Task EnsureStatusCodeAsync(HttpResponseMessage responseMessage, HttpStatusCode expectedStatusCode)
    {
        var responseContent = await responseMessage.Content.ReadAsStringAsync();

        responseMessage.StatusCode.Should().Be(
            expectedStatusCode,
            "request {0} {1} should return the expected status.{2}Response body: {3}",
            responseMessage.RequestMessage?.Method, // - {0}
            responseMessage.RequestMessage?.RequestUri, // - {1}
            Environment.NewLine, // - {2}
            responseContent); // - {3}
    }
}
