using System.Text.Json.Serialization;

namespace DotaMetaExplorer.Models;

public class ProMatches
{
        [JsonPropertyName("match_id")]
        public long MatchId { get; set; }

        [JsonPropertyName("radiant_name")]
        public string? RadiantName { get; set; }

        [JsonPropertyName("dire_name")]
        public string? DireName { get; set; } 

        [JsonPropertyName("radiant_win")]
        public bool RadiantWin { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
}
