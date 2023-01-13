using System.Text.Json.Serialization;

namespace RebrickableSharp.Client;
public class Set
{
    [JsonPropertyName("set_num")] 
    public string SetNum { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("theme_id")]
    public int ThemeId { get; set; }

    [JsonPropertyName("num_parts")]
    public int NumParts { get; set; }

    [JsonPropertyName("set_img_url")]
    public string SetImageURL { get; set; } = null!;

    [JsonPropertyName("set_url")]
    public string SetURL { get; set; } = null!;

    [JsonPropertyName("last_modified_dt")]
    public DateTime? LastModified { get; set; } = null!;
}
