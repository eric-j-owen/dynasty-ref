using System.Text.Json.Serialization;

namespace ClientApi.Dtos.Espn;

public record MappedTeamStats
{
    public required OffenseStats Offense { get; set; }
    public required OppStats Opponent { get; set; }
}

public record OffenseStats
{
    public decimal? TouchdownsPerGame { get; set; }
    public decimal? PassingTouchdownsPerGame { get; set; }
    public decimal? RushingTouchdowns { get; set; }
    public decimal? PassingYardsPerGame { get; set; }
    public decimal? RushingYardsPerGame { get; set; }

}

public record OppStats
{
    public decimal? InterceptionsPerGame { get; set; }
    public decimal? PassesDefendedPerGame { get; set; }
    public decimal? SacksPerGame { get; set; }
    public decimal? TacklesPerGame { get; set; }
    public decimal? TacklesForLossPerGame { get; set; }
    public decimal? StuffsPerGame { get; set; }

}

internal record EspnTeamStatsResponseDto
{
    [JsonPropertyName("results")]
    public required ResultsContainer Results { get; set; }
}

internal record ResultsContainer
{
    [JsonPropertyName("stats")]
    public required StatsContainer Stats { get; set; }

    [JsonPropertyName("opponent")]
    public required List<TeamCategory> Opponent { get; set; }
}

internal record StatsContainer
{
    [JsonPropertyName("categories")]
    public required List<TeamCategory> Categories { get; set; }
}

internal record TeamCategory
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }

    [JsonPropertyName("stats")]
    public required List<TeamStatItem> Stats { get; set; }
}


internal record TeamStatItem
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("perGameValue")]
    public decimal PerGameValue { get; set; }
}