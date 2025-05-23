using System.Text.Json.Serialization;



public class Team
{
    [JsonPropertyName("team_id")]
    public int TeamId { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("wins")]
    public int Wins { get; set; }

    [JsonPropertyName("losses")]
    public int Losses { get; set; }

    [JsonPropertyName("last_match_time")]
    public int LastMatchTime { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("tag")]
    public required string Tag { get; set; }
}
