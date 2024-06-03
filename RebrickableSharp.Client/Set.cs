using RebrickableSharp.Client.Csv;
using System.Text.Json.Serialization;
using CsvPropertyName = CsvHelper.Configuration.Attributes.NameAttribute;

namespace RebrickableSharp.Client;
public class Set : ICsvCompatible
{
    [JsonPropertyName("set_num")]
    [CsvPropertyName("set_num")]
    public string SetNum { get; set; } = null!;

    [JsonPropertyName("name")]
    [CsvPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("year")]
    [CsvPropertyName("year")]
    public int Year { get; set; }

    [CsvPropertyName("theme_id")]
    [JsonPropertyName("theme_id")]
    public int ThemeId { get; set; }

    [JsonPropertyName("num_parts")]
    [CsvPropertyName("num_parts")]
    public int NumParts { get; set; }

    [JsonPropertyName("set_img_url")]
    [CsvPropertyName("img_url")]
    public string SetImageURL { get; set; } = null!;

    [JsonPropertyName("set_url")]
    public string? SetURL { get; set; } = null;

    [JsonPropertyName("last_modified_dt")]
    public DateTime? LastModified { get; set; } = null;
}
