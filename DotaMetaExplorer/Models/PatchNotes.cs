namespace DotaMetaExplorer.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class PatchNotesDto
    {
        [JsonPropertyName("patch_number")]
        public string PatchNumber { get; set; }

        [JsonPropertyName("general_notes")]
        public List<SectionDto> GeneralNotes { get; set; }

        [JsonPropertyName("items")]
        
        public List<SectionDto> Items { get; set; }

        [JsonPropertyName("neutral_items")]

        public List<SectionDto> NeutralItems { get; set; }

        [JsonPropertyName("heroes")]
        public List<HeroDto> Heroes { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class SectionDto
    {
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Title { get; set; }

        [JsonPropertyName("generic")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<NoteDto> Notes { get; set; }
    }

    public class NoteDto
    {
        [JsonPropertyName("indent_level")]
    
        public int IndentLevel { get; set; }

        [JsonPropertyName("note")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Note { get; set; }

        [JsonPropertyName("info")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Info { get; set; }
    }

    public class HeroDto
    {
        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }

        [JsonPropertyName("abilities")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SectionDto> Abilities { get; set; }

        [JsonPropertyName("subsections")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SectionDto> Subsections { get; set; }

        [JsonPropertyName("hero_notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<NoteDto> HeroNotes { get; set; }

        [JsonPropertyName("talent_notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<NoteDto> TalentNotes { get; set; }
    }

}
