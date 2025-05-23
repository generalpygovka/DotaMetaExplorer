using System.Text.Json.Serialization;

public class ProPlayer
{
    [JsonPropertyName("account_id")]
    public int? AccountId { get; set; }

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

    [JsonPropertyName("personaname")]
    public string? PersonaName { get; set; }

    [JsonPropertyName("last_login")]
    public DateTime? LastLogin { get; set; }

    [JsonPropertyName("full_history_time")]
    public DateTime? FullHistoryTime { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("fantasy_role")]
    public int? FantasyRole { get; set; }

    [JsonPropertyName("team_id")]
    public int? TeamId { get; set; }

    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("team_tag")]
    public string? TeamTag { get; set; }
}
