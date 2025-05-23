using System.Text.Json.Serialization;

public class Player
{
    [JsonPropertyName("solo_competitive_rank")]
    public int SoloCompetitiveRank { get; set; }

    [JsonPropertyName("competitive_rank")]
    public int CompetitiveRank { get; set; }

    [JsonPropertyName("rank_tier")]
    public int? RankTier { get; set; }

    [JsonPropertyName("leaderboard_rank")]
    public int? LeaderboardRank { get; set; }

    [JsonPropertyName("profile")]
    public Profile? Profile { get; set; } 
}

public class Profile
{
    [JsonPropertyName("account_id")]
    public int? AccountId { get; set; }

    [JsonPropertyName("personaname")]
    public string? PersonaName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("steamid")]
    public string? SteamId { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("avatarmedium")]
    public string? AvatarMedium { get; set; } 

    [JsonPropertyName("avatarfull")]
    public string? AvatarFull { get; set; }

    [JsonPropertyName("profileurl")]
    public string? ProfileUrl { get; set; } 

    [JsonPropertyName("last_login")]
    public DateTime? LastLogin { get; set; } 

    [JsonPropertyName("loccountrycode")]
    public string? LocCountryCode { get; set; } 
}
