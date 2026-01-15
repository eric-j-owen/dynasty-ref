using System.Text.Json.Serialization;

namespace ClientApi.Dtos.Espn;

public record TeamStatsResponseDto
{
    [JsonPropertyName("results")]
    public ResultsContainer? Results { get; set; }
}

public record ResultsContainer
{
    [JsonPropertyName("stats")]
    public StatsContainer? Stats { get; set; }

    [JsonPropertyName("opponent")]
    public List<TeamCategory>? Opponent { get; set; }
}

public record StatsContainer
{
    [JsonPropertyName("categories")]
    public List<TeamCategory>? Categories { get; set; }
}

public record TeamCategory
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("stats")]
    public List<TeamStatItem>? Stats { get; set; }
}

public record TeamStatItem
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    [JsonPropertyName("value")]
    public decimal? Value { get; set; }
}