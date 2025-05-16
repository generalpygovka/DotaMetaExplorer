using System.Text.Json.Serialization;

namespace DotaMetaExplorer.Models
{
    public class Giphy
    {
        public class RandomGiphy
        {
            [JsonPropertyName("data")]
            public GiphyData? Data { get; set; }
        }
        public class GiphyData
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }
            [JsonPropertyName("images")]
            public Images? Images { get; set; } 
        }
        public class Images
        {
            [JsonPropertyName("fixed_height")]
            public FixedHeight? FixedHeight { get; set; }
        }
        public class FixedHeight
        {
            [JsonPropertyName("url")]
            public string? Url { get; set; }
        }
    }
}
