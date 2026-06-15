using System.Text.Json.Serialization;

namespace Slotegrator.Tools.Models;

public class PlayerResponseDto
{
    // The API is inconsistent: create/delete return "_id", while getOne returns "id".
    [JsonPropertyName("_id")]
    public string? UnderscoreId { get; set; }

    [JsonPropertyName("id")]
    public string? PlainId { get; set; }

    [JsonIgnore]
    public string Id => UnderscoreId ?? PlainId ?? string.Empty;

    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;
}
