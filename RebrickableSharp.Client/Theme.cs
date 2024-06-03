using RebrickableSharp.Client.Csv;
using System.Text.Json.Serialization;
using CsvPropertyName = CsvHelper.Configuration.Attributes.NameAttribute;

namespace RebrickableSharp.Client;
public class Theme : ICsvCompatible
{
    [JsonPropertyName("id")]
    [CsvPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("parent_id")]
    [CsvPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonPropertyName("name")]
    [CsvPropertyName("name")]
    public string Name { get; set; } = default!;
}
