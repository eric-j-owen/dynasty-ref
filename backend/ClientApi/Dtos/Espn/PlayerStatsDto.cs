using System.Text.Json.Serialization;

namespace ClientApi.Dtos.Espn;

public record EspnAthleteStatsResponse
{
    [JsonPropertyName("categories")]
    public required List<StatCategory> Categories { get; set; }
}

public record StatCategory
{
    [JsonPropertyName("name")]
    public required string CategoryName { get; set; }

    [JsonPropertyName("names")]
    public required List<string> StatNames { get; set; }

    [JsonPropertyName("displayNames")]
    public required List<string> StatDisplayNames { get; set; }

    [JsonPropertyName("totals")]
    public required List<string> LastFiveYearsTotals { get; set; }

    [JsonPropertyName("statistics")]
    public required List<YearlyStatObj> CurrentYearTotals { get; set; }
}

public record YearlyStatObj
{
    [JsonPropertyName("stats")]
    public required List<string> Values { get; set; }

    [JsonPropertyName("season")]
    public required SeasonInfo Season { get; set; }
}

public record SeasonInfo
{
    [JsonPropertyName("year")]
    public required int Year { get; set; }
}