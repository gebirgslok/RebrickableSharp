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

namespace RebrickableSharp.Demos;

internal static class ColorDemos
{
    public static async Task GetColorsDemo()
    {
        using var client = RebrickableClientFactory.Build();

        var response1 = await client.GetColorsAsync(includeDetails: true, page: 1, pageSize: 10);

        PrintHelper.PrintAsJson(response1);
        Console.WriteLine();
    }

    public static async Task GetColorDemo()
    {
        using var client = RebrickableClientFactory.Build();

        var colorId = 0; //black
        var black = await client.GetColorAsync(colorId, includeDetails: true);

        PrintHelper.PrintAsJson(black);
        Console.WriteLine();
    }
}
