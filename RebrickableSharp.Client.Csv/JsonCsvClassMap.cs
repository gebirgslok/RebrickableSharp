using CsvHelper.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;

namespace RebrickableSharp.Client.Csv;

/// <summary>
/// Helper class to generate a Csv ClassMap from JsonPropertyName attributes.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class JsonCsvClassMap<T> : ClassMap<T> where T : class
{
    /// <summary>
    /// Creates a new ClassMap for the specified generic type.
    /// </summary>
    /// <param name="alternateNames">a list of alternate names for the properties of T when they do not match JsonPropertyName attributes</param>
    public JsonCsvClassMap(IDictionary<string, string>? alternateNames = null)
    {
        var jsonProperties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() != null);

        foreach (var property in jsonProperties)
        {
            var map = Map(typeof(T), property);
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
            if (jsonPropertyName != null)
            {
                var names = new List<string>
                {
                    jsonPropertyName
                };
                if (alternateNames != null && alternateNames.TryGetValue(property.Name, out var alternateName))
                {
                    names.Add(alternateName);
                }
                map.Name(names.ToArray()).Optional();
            }
            else
            {
                map.Ignore();
            }
        }
    }
}

