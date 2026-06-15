using System.Text.Json.Serialization;

namespace Slotegrator.Tools.Models;

public class TokenDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
}
