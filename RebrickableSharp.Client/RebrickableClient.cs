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

using System.Text.Json;
using System.Web;
using RebrickableSharp.Client.Extensions;

namespace RebrickableSharp.Client;

internal sealed class RebrickableClient : IRebrickableClient
{
    private static readonly Uri _baseUri = new("https://rebrickable.com/api/v3/");
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    private bool _isDisposed;

    public RebrickableClient(HttpClient httpClient, bool disposeHttpClient)
    {
        _httpClient = httpClient;
        _disposeHttpClient = disposeHttpClient;
    }

    ~RebrickableClient()
    {
        Dispose(false);
    }

    private static string AppendApiKey(string url)
    {
        RebrickableClientConfiguration.Instance.ValidateThrowException();

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["key"] = RebrickableClientConfiguration.Instance.ApiKey;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private async Task<TResponse> ExecuteRequest<TResponse>(string url, HttpMethod httpMethod, CancellationToken cancellationToken = default)
    {
        var urlWithKey = AppendApiKey(url);

        using var message = new HttpRequestMessage(httpMethod, urlWithKey);

        message.Content = null;
        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

#if HAVE_HTTP_CONTENT_READ_CANCELLATION_TOKEN
            var contentAsString = await response.Content.ReadAsStringAsync(cancellationToken);
#else
        var contentAsString = await response.Content.ReadAsStringAsync();
#endif

        var responseData = JsonSerializer.Deserialize<TResponse>(contentAsString);

        if (responseData == null)
        {
            //TODO 
            throw new Exception("");
        }

        return responseData;
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
        }

        _isDisposed = true;
    }

    public async Task<PagedResponse<Color>> GetColorsAsync(int page = 1, int pageSize = 100,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(_baseUri, "lego/colors"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        query["inc_color_details"] = includeDetails.ToQueryParam();

        var url = builder.ToString();

        var getColorsResponse = await ExecuteRequest<PagedResponse<Color>>(url, HttpMethod.Get, cancellationToken);
        return getColorsResponse;
    }

    public async Task<Color> GetColorAsync(int colorId, bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(_baseUri, $"lego/colors/{colorId}"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["inc_color_details"] = includeDetails.ToQueryParam();
        var url = builder.ToString();

        var color = await ExecuteRequest<Color>(url, HttpMethod.Get, cancellationToken);
        return color;
    }

    public async Task<PagedResponse<Part>> GetPartsAsync(int page = 1, int pageSize = 100,
        bool includeDetails = false, string? bricklinkId = null,
        string? partNumber = null, IEnumerable<string>? partNumbers = null,
        int? categoryId = null, string? brickOwlId = null,
        string? legoId = null, string? lDrawId = null,
        string? searchTerm = null, 
        CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(_baseUri, "lego/parts"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        query["inc_part_details"] = includeDetails.ToQueryParam();
        query.AddIfNotNull("bricklink_id", bricklinkId);
        query.AddIfNotNull("part_num", partNumber);
        query.AddIfNotNull("part_nums", partNumbers, x => string.Join(",", x!));
        query.AddIfNotNull("part_cat_id", categoryId);
        query.AddIfNotNull("brickowl_id", brickOwlId);
        query.AddIfNotNull("lego_id", legoId);
        query.AddIfNotNull("ldraw_id", lDrawId);
        query.AddIfNotNull("search", searchTerm);
        builder.Query = query.ToString();
        var url = builder.ToString();

        var getPartsResponse = await ExecuteRequest<PagedResponse<Part>>(url, HttpMethod.Get, cancellationToken);
        return getPartsResponse;
    }

    public async Task<Part?> FindPartByBricklinkIdAsync(string bricklinkId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var response = await GetPartsAsync(pageSize: 1, includeDetails:
            includeDetails, bricklinkId: bricklinkId, cancellationToken: cancellationToken);

        return response.Results.FirstOrDefault();
    }

    public async Task<PartColorDetails> GetPartColorDetailsAsync(string partNumber, int colorId, 
        CancellationToken cancellationToken = default)
    {
        var url = new Uri(_baseUri, $"lego/parts/{partNumber}/colors/{colorId}").ToString();
        var partColorDetails = await ExecuteRequest<PartColorDetails>(url, HttpMethod.Get, cancellationToken);
        return partColorDetails;
    }

    public async Task<Element> GetElementAsync(string elementId, CancellationToken cancellationToken = default)
    {
        var url = new Uri(_baseUri, $"lego/elements/{elementId}").ToString();
        var element = await ExecuteRequest<Element>(url, HttpMethod.Get, cancellationToken);
        return element;
    }

    public async Task<PagedResponse<SetPart>> GetSetPartsAsync(string id,
    int page = 1, int pageSize = 100,
    CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(_baseUri, $"lego/sets/{id}/parts"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        builder.Query = query.ToString();
        var url = builder.ToString();

        var getSetPartsResponse = await ExecuteRequest<PagedResponse<SetPart>>(url, HttpMethod.Get, cancellationToken);
        return getSetPartsResponse;
    }

    public async Task<PagedResponse<Set>> GetSetsAsync(int minYear, int maxYear,
         int minParts = 0, int maxParts = 100000,
         int page = 1, int pageSize = 100,
         CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(_baseUri, "lego/sets"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        query["min_year"] = minYear.ToString();
        query["max_year"] = maxYear.ToString();
        query["min_parts"] = minParts.ToString();
        query["max_parts"] = maxParts.ToString();
        builder.Query = query.ToString();
        var url = builder.ToString();

        var getSetsResponse = await ExecuteRequest<PagedResponse<Set>>(url, HttpMethod.Get, cancellationToken);
        return getSetsResponse;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}