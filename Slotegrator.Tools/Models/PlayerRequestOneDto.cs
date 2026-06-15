using System.Text.Json.Serialization;

namespace Slotegrator.Tools.Models;

public class PlayerRequestOneDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
