using System.Text.Json.Serialization;

namespace RebrickableSharp.Client;
public class Theme
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
}
