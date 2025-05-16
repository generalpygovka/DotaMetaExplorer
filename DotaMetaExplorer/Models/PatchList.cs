namespace DotaMetaExplorer.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class PatchList
    {
        [JsonPropertyName("patches")]
        public List<PatchInfo>? Patches { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class PatchInfo
    {
        [JsonPropertyName("patch_number")]
        public string? PatchNumber { get; set; }
    }

}
