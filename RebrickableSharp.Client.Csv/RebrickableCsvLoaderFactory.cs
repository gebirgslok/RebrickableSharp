using RebrickableSharp.Client.Csv;

namespace RebrickableSharp.Client;

public static class RebrickableCsvLoaderFactory
{
    private static IRebrickableCsvLoader Build(HttpClient httpClient, bool disposeHttpClient, string baseUriString = RebrickableCsvLoader.DefaultBaseUri)
    {
        return new RebrickableCsvLoader(httpClient, disposeHttpClient, baseUriString);
    }

    public static IRebrickableCsvLoader Build(HttpClient httpClient, string baseUriString = RebrickableCsvLoader.DefaultBaseUri)
    {
        return Build(httpClient, false, baseUriString);
    }

    public static IRebrickableCsvLoader Build()
    {
        return Build(new HttpClient(), true, RebrickableCsvLoader.DefaultBaseUri);
    }
}