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
    public int AccountId { get; set; }

    [JsonPropertyName("personaname")]
    public string? PersonaName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("plus")]
    public bool Plus { get; set; }

    [JsonPropertyName("cheese")]
    public int Cheese { get; set; }

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
    public string? LastLogin { get; set; } 

    [JsonPropertyName("loccountrycode")]
    public string? LocCountryCode { get; set; } 

    [JsonPropertyName("is_contributor")]
    public bool IsContributor { get; set; }

    [JsonPropertyName("is_subscriber")]
    public bool IsSubscriber { get; set; }
}
