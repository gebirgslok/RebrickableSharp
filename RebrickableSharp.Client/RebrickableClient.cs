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
    private readonly IRebrickableRequestHandler? _requestHandler;

    private bool _isDisposed;

    public RebrickableClient(
        HttpClient httpClient,
        bool disposeHttpClient,
        IRebrickableRequestHandler? requestHandler = null
    )
    {
        _httpClient = httpClient;
        _disposeHttpClient = disposeHttpClient;
        _requestHandler = requestHandler;
    }

    ~RebrickableClient()
    {
        Dispose(false);
    }

    private string EnsureApiKey(RebrickableCredentials? credentials)
    {
        var apiKey =
            credentials != null
                ? credentials.ApiKey
                : RebrickableClientConfiguration.Instance.ApiKey;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new RebrickableMissingCredentialsException(
                new[] { nameof(RebrickableCredentials.ApiKey) }
            );
        }

        return apiKey!;
    }

    private async Task MeasureRequestAsync(
        RebrickableApiResourceType resourceType,
        HttpVerb verb,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        if (_requestHandler != null)
        {
            await _requestHandler.OnRequestAsync(resourceType, verb, apiKey, cancellationToken);
        }
    }

    private static string AppendApiKey(string url, string apiKey)
    {
        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["key"] = apiKey;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private async Task<TResponse> ExecuteRequest<TResponse>(
        string url,
        HttpMethod httpMethod,
        RebrickableCredentials? credentials,
        RebrickableApiResourceType resourceType,
        CancellationToken cancellationToken = default
    )
    {
        var apiKey = EnsureApiKey(credentials);
        var urlWithKey = AppendApiKey(url, apiKey);

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
            throw new InvalidOperationException(
                "Failed to deserialize response content (responseData is Null)."
            );
        }

        await MeasureRequestAsync(resourceType, HttpVerb.Get, apiKey, cancellationToken);

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

    public async Task<PagedResponse<Theme>> GetThemesAsync(
        int page = 1,
        int pageSize = 100,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var uriBuilder = new UriBuilder(new Uri(_baseUri, "lego/themes/"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        uriBuilder.Query = query.ToString();
        var url = uriBuilder.ToString();

        var getThemesResponse = await ExecuteRequest<PagedResponse<Theme>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Theme,
            cancellationToken
        );

        return getThemesResponse;
    }

    public async Task<PagedResponse<Color>> GetColorsAsync(
        int page = 1,
        int pageSize = 100,
        bool includeDetails = false,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var uriBuilder = new UriBuilder(new Uri(_baseUri, "lego/colors"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        query["inc_color_details"] = includeDetails.ToQueryParam();
        uriBuilder.Query = query.ToString();
        var url = uriBuilder.ToString();

        var getColorsResponse = await ExecuteRequest<PagedResponse<Color>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Color,
            cancellationToken
        );

        return getColorsResponse;
    }

    public async Task<Color> GetColorAsync(
        int colorId,
        bool includeDetails = false,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var builder = new UriBuilder(new Uri(_baseUri, $"lego/colors/{colorId}"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["inc_color_details"] = includeDetails.ToQueryParam();
        var url = builder.ToString();

        var color = await ExecuteRequest<Color>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Color,
            cancellationToken
        );

        return color;
    }

    public async Task<PagedResponse<Part>> GetPartsAsync(
        int page = 1,
        int pageSize = 100,
        bool includeDetails = false,
        string? bricklinkId = null,
        string? partNumber = null,
        IEnumerable<string>? partNumbers = null,
        int? categoryId = null,
        string? brickOwlId = null,
        string? legoId = null,
        string? lDrawId = null,
        string? searchTerm = null,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
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

        var getPartsResponse = await ExecuteRequest<PagedResponse<Part>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Part,
            cancellationToken
        );

        return getPartsResponse;
    }

    public async Task<Part?> FindPartByBricklinkIdAsync(
        string bricklinkId,
        bool includeDetails = false,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var response = await GetPartsAsync(
            pageSize: 1,
            includeDetails: includeDetails,
            bricklinkId: bricklinkId,
            credentials: credentials,
            cancellationToken: cancellationToken
        );

        return response.Results.FirstOrDefault();
    }

    public async Task<PartColorDetails> GetPartColorDetailsAsync(
        string partNumber,
        int colorId,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var url = new Uri(_baseUri, $"lego/parts/{partNumber}/colors/{colorId}").ToString();

        var partColorDetails = await ExecuteRequest<PartColorDetails>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Part,
            cancellationToken
        );

        return partColorDetails;
    }

    public async Task<Element> GetElementAsync(
        string elementId,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var url = new Uri(_baseUri, $"lego/elements/{elementId}").ToString();

        var element = await ExecuteRequest<Element>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Element,
            cancellationToken
        );

        return element;
    }

    public async Task<PagedResponse<Minifig>> GetMinifigsAsync(
        int page = 1,
        int pageSize = 100,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var builder = new UriBuilder(new Uri(_baseUri, "lego/minifigs"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        query["inc_part_details"] = true.ToQueryParam();
        builder.Query = query.ToString();
        var url = builder.ToString();

        var getMinifigsResponse = await ExecuteRequest<PagedResponse<Minifig>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Minifigure,
            cancellationToken
        );

        return getMinifigsResponse;
    }

    public Task<Minifig> GetMinifigByIdAsync(
        string minifigId,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    ) =>
        ExecuteRequest<Minifig>(
            new UriBuilder(new Uri(_baseUri, $"lego/minifigs/{minifigId}")).ToString(),
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Minifigure,
            cancellationToken
        );

    public async Task<PagedResponse<SetPart>> GetSetPartsAsync(
        string id,
        int page = 1,
        int pageSize = 100,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
    {
        var builder = new UriBuilder(new Uri(_baseUri, $"lego/sets/{id}/parts"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();
        builder.Query = query.ToString();
        var url = builder.ToString();

        var getSetPartsResponse = await ExecuteRequest<PagedResponse<SetPart>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Set,
            cancellationToken
        );

        return getSetPartsResponse;
    }

    public async Task<PagedResponse<Set>> GetSetsAsync(
        int minYear,
        int maxYear,
        int minParts = 0,
        int maxParts = 100000,
        int page = 1,
        int pageSize = 100,
        RebrickableCredentials? credentials = null,
        CancellationToken cancellationToken = default
    )
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

        var getSetsResponse = await ExecuteRequest<PagedResponse<Set>>(
            url,
            HttpMethod.Get,
            credentials,
            RebrickableApiResourceType.Set,
            cancellationToken
        );

        return getSetsResponse;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
