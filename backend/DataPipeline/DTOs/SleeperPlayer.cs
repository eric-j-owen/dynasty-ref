using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public class SleeperPlayer
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
}





