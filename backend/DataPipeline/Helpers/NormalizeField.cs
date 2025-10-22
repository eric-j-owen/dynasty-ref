using System.Net;
using System.Text.RegularExpressions;

namespace DataPipeline.Helpers;

public static class NormalizeField
{
    public static string Name(string input)
    {
        var decoded = WebUtility.HtmlDecode(input);
        return MakeAlphabetic(decoded).ToLower();
    }

    public static string Position(string input)
    {
        return MakeAlphabetic(input);
    }

    private static string MakeAlphabetic(string input)
    {
        Regex rgx = new Regex("[^a-zA-Z]");
        input = rgx.Replace(input, "");
        return input;
    }
}