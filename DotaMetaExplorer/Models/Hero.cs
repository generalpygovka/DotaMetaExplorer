using System.Text.Json.Serialization;

namespace DotaMetaExplorer.Models
{
    public class Hero
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("localized_name")]
        public required string LocalizedName { get; set; }

        [JsonPropertyName("primary_attr")]
        public required string PrimaryAttr { get; set; }

        [JsonPropertyName("attack_type")]
        public required string AttackType { get; set; }

        [JsonPropertyName("roles")]
        public required string[] Roles { get; set; }
    }
}
