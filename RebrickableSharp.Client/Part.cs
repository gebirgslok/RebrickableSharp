#region License
// Copyright (c) 2022 Jens Eisenbach
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.Text.Json.Serialization;

namespace RebrickableSharp.Client;

public class Part
{
    [JsonPropertyName("part_num")]
    public string PartNumber { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("part_cat_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("part_url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("part_img_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("year_from")]
    public int? YearFrom { get; set; }

    [JsonPropertyName("year_to")]
    public int? YearTo { get; set; }

    [JsonPropertyName("molds")]
    public string[] Molds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("prints")]
    public string[] Prints { get; set; } = Array.Empty<string>();

    [JsonPropertyName("alternates")]
    public string[] Alternates { get; set; } = Array.Empty<string>();

    [JsonPropertyName("external_ids")]
    public ExternalPartIds ExternalIds { get; set; } = new ExternalPartIds();

    [JsonPropertyName("print_of")]
    public string? PrintOf { get; set; }

    public override string ToString()
    {
        return $"{PartNumber} - {Name}";
    }
}