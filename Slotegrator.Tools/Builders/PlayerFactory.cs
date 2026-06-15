using Slotegrator.Tools.Models;

namespace Slotegrator.Tools.Builders;

public static class PlayerFactory
{
    public static PlayerRequestDto CreatePlayer() => new PlayerBuilder().Build();

    public static List<PlayerRequestDto> CreatePlayers(int playersCount)
        => Enumerable.Range(1, playersCount)
            .Select(_ => CreatePlayer())
            .ToList();
}
