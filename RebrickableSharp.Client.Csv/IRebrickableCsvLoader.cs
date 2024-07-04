namespace RebrickableSharp.Client.Csv;

public interface IRebrickableCsvLoader : IDisposable
{
    /// <summary>
    /// Donwloads and parses a Csv from file from the Rebrickable CDN into an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <returns>an array of <typeparamref name="T"/></returns>
    /// <param name="cancellationToken">an optional CancellationToken for the http, decompress and parsing of the Csv file</param>
    /// <exception cref="RebrickableCsvException">if the download fails</exception>
    Task<T[]> DownloadAsync<T>(CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Downloads a Csv static file from the Rebrickable CDN.
    /// </summary>
    /// <typeparam name="T">the type of Csv file to download</typeparam>
    /// <param name="decompress">whether to decompress the file after download</param>
    /// <param name="cancellationToken">an optional CancellationToken for the http and decompress of the Csv file</param>
    /// <returns>a temp filename containg the result of the download</returns>
    Task<string?> DownloadFileAsync<T>(
        bool decompress = true,
        CancellationToken cancellationToken = default
    )
        where T : class;

    /// <summary>
    /// Parses records from a Csv file to an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <param name="csvFileName">The Csv file to parse</param>
    /// <param name="cancellationToken">an optional CancellationToken for the Csv parsing of the file</param>
    /// <returns>an array of <typeparamref name="T"/></returns>
    Task<T[]> ParseAsync<T>(string csvFileName, CancellationToken cancellationToken = default)
        where T : class;
}
