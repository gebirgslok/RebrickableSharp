using System.Globalization;
using System.IO.Compression;
using CsvHelper;
using CsvHelper.Configuration;

namespace RebrickableSharp.Client.Csv;

/// <summary>
/// <para>
/// A class to download and parse Csv static files from the Rebrickable download CDN.
/// </para>
/// <para>See <see cref="https://rebrickable.com/downloads/"/></para>
/// </summary>
public class RebrickableCsvLoader : IRebrickableCsvLoader
{
    public const string DefaultBaseUri = "https://cdn.rebrickable.com/media/downloads/";

    private readonly HttpClient _httpClient;
    private bool _disposed;
    private bool _disposeHttpClient;
    private readonly Uri _downloadBaseUri;

    public RebrickableCsvLoader(
        HttpClient? httpClient = null,
        bool disposeHttpClient = true,
        string baseUriString = DefaultBaseUri
    )
    {
        _httpClient = httpClient ?? new HttpClient();
        _disposeHttpClient = disposeHttpClient;
        _downloadBaseUri = new Uri(baseUriString);
    }

    /// <summary>
    /// Parses records from an opened Csv StreamReader to an array of T.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <param name="csvFileName">The StreamReader to read Csv records from</param>
    /// <returns>an array of T</returns>
    private async Task<T[]> ParseStreamAsync<T>(
        StreamReader reader,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap(GetClassMap<T>());
        var records = csv.GetRecordsAsync<T>(cancellationToken);
        return await records.ToArrayAsync();
    }

    private ClassMap GetClassMap<T>()
    {
        if (typeof(T) == typeof(Set))
        {
            var alternateNames = new Dictionary<string, string>
            {
                { nameof(Set.SetImageURL), "img_url" }
            };
            return new JsonCsvClassMap<Set>(alternateNames);
        }
        else if (typeof(T) == typeof(Theme))
        {
            return new JsonCsvClassMap<Theme>();
        }
        else
        {
            throw new NotImplementedException(
                $"{nameof(GetClassMap)} for {typeof(T).FullName} is not implemented"
            );
        }
    }

    /// <inheritdoc />
    public async Task<T[]> ParseAsync<T>(
        string csvFileName,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        using var reader = new StreamReader(csvFileName);
        return await ParseStreamAsync<T>(reader, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> DownloadFileAsync<T>(
        bool decompress = true,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        return await DownloadFileAsync(GetDownloadUri<T>(), decompress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T[]> DownloadAsync<T>(CancellationToken cancellationToken = default)
        where T : class
    {
        string? csvFileName = null;
        var uri = GetDownloadUri<T>();
        try
        {
            csvFileName = await DownloadFileAsync(uri, true, cancellationToken);
            if (!File.Exists(csvFileName))
            {
                throw new RebrickableCsvException(
                    $"Failed to download a file for {typeof(T)} from {uri}"
                );
            }
            return await ParseAsync<T>(csvFileName, cancellationToken);
        }
        finally
        {
            if (csvFileName != null)
            {
                File.Delete(csvFileName);
            }
        }
    }

    private static async Task<string> DecompressFileAsync(
        string gzipFileName,
        CancellationToken cancellationToken = default
    )
    {
        var path = Path.GetTempFileName();
        using (
            var originalFileStream = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read)
        )
        {
            using (var decompressedFileStream = File.Create(path))
            {
                using (
                    var decompressionStream = new GZipStream(
                        originalFileStream,
                        CompressionMode.Decompress
                    )
                )
                {
#if HAVE_STREAM_COPY_TO_ASYNC_CANCELLATION_TOKEN
                    await decompressionStream.CopyToAsync(
                        decompressedFileStream,
                        cancellationToken
                    );
#else
                    await decompressionStream.CopyToAsync(decompressedFileStream);
#endif
                }
            }
        }
        return path;
    }

    private Uri GetDownloadUri<T>()
        where T : class
    {
        string resource;
        if (typeof(T) == typeof(Set))
        {
            resource = "sets";
        }
        else if (typeof(T) == typeof(Theme))
        {
            resource = "themes";
        }
        else
        {
            throw new NotImplementedException(
                $"Download uri for {typeof(T).FullName} is not implemented"
            );
        }
        return new Uri(_downloadBaseUri, resource + ".csv.gz");
    }

    private async Task<string> DownloadFileAsync(
        Uri uri,
        bool decompress = true,
        CancellationToken cancellationToken = default
    )
    {
        string? outputFile = null;
        string? tempFile = null;
        try
        {
            using (var response = await _httpClient.GetAsync(uri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                tempFile = Path.GetTempFileName();
                using (
                    var fileStream = new FileStream(
                        tempFile,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        8192,
                        true
                    )
                )
                {
#if HAVE_HTTP_CONTENT_COPY_TO_CANCELLATION_TOKEN
                    await response.Content.CopyToAsync(fileStream, cancellationToken);
#else
                    await response.Content.CopyToAsync(fileStream);
#endif
                }
                if (decompress)
                {
                    outputFile = await DecompressFileAsync(tempFile, cancellationToken);
                }
                else
                {
                    outputFile = tempFile;
                }
            }
        }
        finally
        {
            if (decompress && outputFile != null && tempFile != null)
            {
                File.Delete(tempFile);
            }
        }

        return outputFile;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_disposeHttpClient)
                {
                    _httpClient?.Dispose();
                }
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
