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

namespace RebrickableSharp.Client;

public interface IRebrickableClient : IDisposable
{
    Task<PagedResponse<Color>> GetColorsAsync(int page = 1, int pageSize = 100,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<Color> GetColorAsync(int colorId, bool includeDetails = false, 
        CancellationToken cancellationToken = default);

    Task<PagedResponse<Part>> GetPartsAsync(int page = 1, int pageSize = 100, 
        bool includeDetails = false, string? bricklinkId = null,
        string? partNumber = null, IEnumerable<string>? partNumbers = null,
        int? categoryId = null, string? brickOwlId = null,
        string? legoId = null, string? lDrawId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<Part?> FindPartByBricklinkIdAsync(string bricklinkId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<PartColorDetails> GetPartColorDetailsAsync(string partNumber, int colorId,
        CancellationToken cancellationToken = default);

    Task<Element> GetElementAsync(string elementId, 
        CancellationToken cancellationToken = default);


    Task<PagedResponse<SetPart>> GetSetPartsAsync(string id,
        int page = 1, int pageSize = 100,
        CancellationToken cancellationToken = default);


}