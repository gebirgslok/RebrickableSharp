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

public class ExternalPartIds
{
    private class ExternalPartIdStringBuilder
    {
        private readonly IDictionary<string, int> _externalIdCount = new Dictionary<string, int>();

        public void Append(string[] ids, string externalSource)
        {
            if (ids.Any())
            {
                _externalIdCount.Add(externalSource, ids.Length);
            }
        }

        public override string ToString()
        {
            if (!_externalIdCount.Any())
            {
                return "No external IDs available";
            }

            return string.Join(",", _externalIdCount.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        }
    }

    [JsonPropertyName("BrickLink")]
    public string[] BricklinkIds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("BrickOwl")]
    public string[] BrickOwlIds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("Brickset")]
    public string[] BricksetIds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("LEGO")]
    public string[] LegoIds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("LDraw")]
    public string[] LDrawIds { get; set; } = Array.Empty<string>();

    [JsonPropertyName("Peeron")]
    public string[] PeeronIds { get; set; } = Array.Empty<string>();

    public override string ToString()
    {
        var builder = new ExternalPartIdStringBuilder();
        builder.Append(BricklinkIds, "BrickLink");
        builder.Append(BrickOwlIds, "BrickOwl");
        builder.Append(BricksetIds, "Brickset");
        builder.Append(LegoIds, "LEGO");
        builder.Append(LDrawIds, "LDraw");
        builder.Append(PeeronIds, "Peeron");
        return builder.ToString();
    }
}