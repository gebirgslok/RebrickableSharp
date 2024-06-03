using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO.Compression;

namespace RebrickableSharp.Client.Csv;

/// <summary>
/// <para>
/// A class to download and parse Csv static files from the Rebrickable download CDN.
/// </para>
/// <para>See <see cref="https://rebrickable.com/downloads/"/></para>
/// </summary>
public class RebrickableCsvLoader : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed;
    private bool _disposeHttpClient;
    private readonly Uri _downloadBaseUri;

    private const string DefaultBaseUri = "https://cdn.rebrickable.com/media/downloads/";

    public RebrickableCsvLoader(HttpClient? httpClient = null, string baseUriString = DefaultBaseUri)
    {
        _httpClient = httpClient ?? new HttpClient();
        _disposeHttpClient = httpClient == null;
        _downloadBaseUri = new Uri(baseUriString);
    }

    /// <summary>
    /// Parses records from an opened Csv StreamReader to an array of T.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <param name="csvFileName">The StreamReader to read Csv records from</param>
    /// <returns>an array of T</returns>
    public async Task<T[]> ParseStreamAsync<T>(StreamReader reader) where T : class, ICsvCompatible
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",            
            TrimOptions = TrimOptions.Trim,
            HeaderValidated = null,
            MissingFieldFound = null,
        };
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecordsAsync<T>();
        return await records.ToArrayAsync();
    }

    /// <summary>
    /// Parses records from a Csv file to an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <param name="csvFileName">The Csv file to parse</param>
    /// <returns>an array of <typeparamref name="T"/></returns>
    public async Task<T[]> ParseAsync<T>(string csvFileName) where T : class, ICsvCompatible
    {
        using var reader = new StreamReader(csvFileName);
        return await ParseStreamAsync<T>(reader);
    }

    private async Task<string> DecompressFileAsync(string gzipFileName)
    {
        var path = Path.GetTempFileName();
        using (var originalFileStream = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read))
        {
            using (var decompressedFileStream = File.Create(path))
            {
                using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    await decompressionStream.CopyToAsync(decompressedFileStream);
                }
            }
        }
        return path;
    }

    /// <summary>
    /// Downloads a Csv static file from the rebrickable CDN.
    /// </summary>
    /// <typeparam name="T">the type of Csv file to download</typeparam>
    /// <param name="decompress">whether to decompress the file after download</param>
    /// <returns>a temp filename containg the result of the download</returns>
    public async Task<string?> DownloadFileAsync<T>(bool decompress = true) where T : class, ICsvCompatible
    {
        return await DownloadFileAsync(GetDownloadUri<T>(), decompress);
    }

    /// <summary>
    /// Donwloads and parses a Csv from file from the Rebrickable CDN into an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <returns>an array of <typeparamref name="T"/></returns>
    /// <exception cref="RebrickableCsvException">if the download fails</exception>
    public async Task<T[]> DownloadAsync<T>() where T : class, ICsvCompatible
    {
        string? csvFileName = null;
        var uri = GetDownloadUri<T>();
        try
        {
            csvFileName = await DownloadFileAsync(uri);
            if (!File.Exists(csvFileName))
            {
                throw new RebrickableCsvException($"Failed to dowload a file for {typeof(T)} from {uri}");
            }
            return await ParseAsync<T>(csvFileName);
        }
        finally
        {
            if (csvFileName != null)
            {
                File.Delete(csvFileName);
            }
        }
    }

    private Uri GetDownloadUri<T>() where T : class, ICsvCompatible
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
            throw new NotImplementedException($"Download uri for {typeof(T)} is not implemented");
        }
        return new Uri(_downloadBaseUri, resource + ".csv.gz");
    }

    private async Task<string> DownloadFileAsync(Uri uri, bool decompress = true) {
        string? outputFile = null;
        string? tempFile = null;
        try {
            using (var response = await _httpClient.GetAsync(uri))
            {
                response.EnsureSuccessStatusCode();
                byte[] fileContents = await response.Content.ReadAsByteArrayAsync();

                tempFile = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
                if (decompress)
                {
                    outputFile = await DecompressFileAsync(tempFile);
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
