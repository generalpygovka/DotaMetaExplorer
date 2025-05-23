using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace DotaMetaExplorer.Models;
public class PatchNotes
{
    [JsonPropertyName("patch_number")]
    public string PatchNumber { get; set; }

    [JsonPropertyName("patch_name")]
    public string PatchName { get; set; }

    [JsonPropertyName("patch_timestamp")]
    public long PatchTimestamp { get; set; }

    [JsonPropertyName("general_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GeneralNoteSection>? GeneralNotes { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilitySection>? Items { get; set; }

    [JsonPropertyName("neutral_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilitySection>? NeutralItems { get; set; }

    [JsonPropertyName("heroes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<HeroSection>? Heroes { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }
}

public class GeneralNoteSection
{
    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; }

    [JsonPropertyName("generic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<NoteEntry>? Generic { get; set; }
}

public class NoteEntry
{
    [JsonPropertyName("indent_level")]
    public int IndentLevel { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; }
}

public class AbilitySection
{
    [JsonPropertyName("ability_id")]
    public int AbilityId { get; set; }

    [JsonPropertyName("postfix_lines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PostfixLines { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; }

    [JsonPropertyName("is_general_note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsGeneralNote { get; set; }

    [JsonPropertyName("ability_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilityNote>? AbilityNotes { get; set; }
}

public class AbilityNote
{
    [JsonPropertyName("indent_level")]
    public int IndentLevel { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; }

    [JsonPropertyName("info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Info { get; set; }
}

public class HeroSection
{
    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }

    [JsonPropertyName("subsections")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<HeroSubsection>? Subsections { get; set; }

    [JsonPropertyName("abilities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilitySection>? Abilities { get; set; }

    [JsonPropertyName("hero_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilityNote>? HeroNotes { get; set; }

    [JsonPropertyName("talent_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilityNote>? TalentNotes { get; set; }
}

public class HeroSubsection
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("style")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Style { get; set; }

    [JsonPropertyName("facet")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Facet { get; set; }

    [JsonPropertyName("facet_icon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FacetIcon { get; set; }

    [JsonPropertyName("facet_color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FacetColor { get; set; }

    [JsonPropertyName("general_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilityNote>? GeneralNotes { get; set; }

    [JsonPropertyName("abilities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilitySection>? Abilities { get; set; }

    [JsonPropertyName("talent_notes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AbilityNote>? TalentNotes { get; set; }
}
