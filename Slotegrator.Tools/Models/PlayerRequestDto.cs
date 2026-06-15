using System.Text.Json.Serialization;

namespace Slotegrator.Tools.Models;

public class PlayerRequestDto
{
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("password_change")]
    public string PasswordChange { get; set; } = string.Empty;

    [JsonPropertyName("password_repeat")]
    public string PasswordRepeat { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}
