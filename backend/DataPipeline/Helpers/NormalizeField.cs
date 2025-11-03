using System.Net;
using System.Text.RegularExpressions;
using Shared.Consts;

namespace DataPipeline.Helpers;

public static class NormalizeField
{
    public static string Name(string input)
    {
        var decoded = WebUtility.HtmlDecode(input);
        return MakeAlphabetic(decoded).ToLower();
    }

    public static IncludedPosition Position(string input)
    {
        var normalized = MakeAlphabetic(input);
        Enum.TryParse<IncludedPosition>(normalized, true, out var position);
        return position;
    }

    public static TeamAbbr Team(string? input) => input switch
    {
        "SFO" => TeamAbbr.SF,
        "NOS" => TeamAbbr.NO,
        "JAC" => TeamAbbr.JAX,
        "GB" => TeamAbbr.GB,
        "KC" => TeamAbbr.KC,
        "NE" => TeamAbbr.NE,
        "TB" => TeamAbbr.TB,
        "LV" => TeamAbbr.LV,
        "FA" => TeamAbbr.NullTeam,
        _ => Enum.TryParse<TeamAbbr>(input, true, out var team) ? team : TeamAbbr.NullTeam
    };

    private static string MakeAlphabetic(string input)
    {
        Regex rgx = new Regex("[^a-zA-Z]");
        input = rgx.Replace(input, "");
        return input;
    }
}