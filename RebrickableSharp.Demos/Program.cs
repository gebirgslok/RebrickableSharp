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

using RebrickableSharp.Client;
using RebrickableSharp.Client.Csv;

namespace RebrickableSharp.Demos;

internal static class Program
{
    static async Task<int> Main(string[] args)
    {
        var jsonDemo = args.Length == 0 || args[0]?.Equals("csv", StringComparison.InvariantCultureIgnoreCase) != true;
        if (jsonDemo)
        {
            return await JsonDemo();
        }
        else
        {
            return await CsvDemo();
        }
    }

    static async Task<int> JsonDemo()
    {
        RebrickableClientConfiguration.Instance.ApiKey = Environment.GetEnvironmentVariable("REBRICKABLE_API_KEY") ?? "<YOUR API KEY>";
        //await PartDemos.GetPartsDemo();
        await PartDemos.GetPartsTestGithubIssue1();
        //await PartDemos.FindPartByBrickLinkIdDemo();
        //await PartDemos.GetPartColorDetailsDemo();
        //await ColorDemos.GetColorsDemo();
        //await ColorDemos.GetColorDemo();
        //await ElementDemos.GetElementDemo();
        //await MinifigDemos.GetMinifigsDemo();
        Console.ReadKey();
        return 0;
    }

    static async Task<int> CsvDemo()
    {
        var csvLoader = RebrickableCsvLoaderFactory.Build();
        var result = await csvLoader.DownloadAsync<Theme>();
        //var result = await csvLoader.ParseAsync<Set>(csvSetFileName);
        PrintHelper.PrintAsJson(result);
        Console.ReadKey();
        return 0;
    }
}