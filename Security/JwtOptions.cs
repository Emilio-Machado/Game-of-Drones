namespace Game_of_Drones.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int LifetimeMinutes { get; init; } = 60;
}

public static class GameClaims
{
    public const string GameId = "game_id";
}
