using System.Text.RegularExpressions;

namespace WebScraping.Helpers;

public static class NormalizeField
{
    public static string Name(string input)
    {
        return MakeAlphabetic(input).ToLower();
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