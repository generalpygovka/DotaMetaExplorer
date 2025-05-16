using System.Text.Json.Serialization;

namespace DotaMetaExplorer.Models
{
    public class ProPlayer
    {
        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("steamid")]
        public required string SteamId { get; set; }

        [JsonPropertyName("avatar")]
        public required string Avatar { get; set; }

        [JsonPropertyName("avatarmedium")]
        public required string AvatarMedium { get; set; }

        [JsonPropertyName("avatarfull")]
        public required string AvatarFull { get; set; }

        [JsonPropertyName("profileurl")]
        public required string ProfileUrl { get; set; }

        [JsonPropertyName("personaname")]
        public required string PersonaName { get; set; }

        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; set; }

        [JsonPropertyName("full_history_time")]
        public DateTime? FullHistoryTime { get; set; }

        [JsonPropertyName("cheese")]
        public int? Cheese { get; set; }

        [JsonPropertyName("fh_unavailable")]
        public bool? FullHistoryUnavailable { get; set; }

        [JsonPropertyName("loccountrycode")]
        public required string LocCountryCode { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("country_code")]
        public required string CountryCode { get; set; }

        [JsonPropertyName("fantasy_role")]
        public int FantasyRole { get; set; }

        [JsonPropertyName("team_id")]
        public int TeamId { get; set; }

        [JsonPropertyName("team_name")]
        public required string TeamName { get; set; }

        [JsonPropertyName("team_tag")]
        public required string TeamTag { get; set; }

        [JsonPropertyName("is_locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("is_pro")]
        public bool IsPro { get; set; }

        [JsonPropertyName("locked_until")]
        public int? LockedUntil { get; set; }
    }
}
