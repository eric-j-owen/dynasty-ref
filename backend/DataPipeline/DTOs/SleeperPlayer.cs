using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public record SleeperPlayerDto
{
    [JsonPropertyName("player_id")]
    public string? SleeperId { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("search_full_name")]
    public string? SearchFullName { get; set; }

    [JsonPropertyName("fantasy_positions")]
    public string[]? Positions { get; set; }

    [JsonPropertyName("team")]
    public string? Team { get; set; }

    [JsonPropertyName("injury_status")]
    public string? InjuryStatus { get; set; }

    [JsonPropertyName("college")]
    public string? College { get; set; }


    [JsonPropertyName("age")]
    public int? Age { get; set; }

}





