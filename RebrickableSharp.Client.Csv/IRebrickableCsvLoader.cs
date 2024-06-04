namespace RebrickableSharp.Client.Csv;

public interface IRebrickableCsvLoader : IDisposable
{
    /// <summary>
    /// Donwloads and parses a Csv from file from the Rebrickable CDN into an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <returns>an array of <typeparamref name="T"/></returns>
    /// <exception cref="RebrickableCsvException">if the download fails</exception>
    Task<T[]> DownloadAsync<T>() where T : class;

    /// <summary>
    /// Downloads a Csv static file from the Rebrickable CDN.
    /// </summary>
    /// <typeparam name="T">the type of Csv file to download</typeparam>
    /// <param name="decompress">whether to decompress the file after download</param>
    /// <returns>a temp filename containg the result of the download</returns>
    Task<string?> DownloadFileAsync<T>(bool decompress = true) where T : class;

    /// <summary>
    /// Parses records from a Csv file to an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A Rebrickable Csv compatible type</typeparam>
    /// <param name="csvFileName">The Csv file to parse</param>
    /// <returns>an array of <typeparamref name="T"/></returns>
    Task<T[]> ParseAsync<T>(string csvFileName) where T : class;
}