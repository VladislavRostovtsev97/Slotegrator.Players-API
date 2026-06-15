namespace Slotegrator.Tools.Constants;

public static class Endpoints
{
    public static readonly string LoginUrl = "api/tester/login";
    public static readonly string CreatePlayerUrl = "api/automationTask/create";
    public static readonly string GetPlayerUrl = "api/automationTask/getOne";
    public static readonly string GetAllPlayersUrl = "api/automationTask/getAll";
    public static readonly string DeletePlayerBaseUrl = "api/automationTask/deleteOne";

    public static string DeletePlayerUrl(string playerId) => $"{DeletePlayerBaseUrl}/{playerId}";
}
