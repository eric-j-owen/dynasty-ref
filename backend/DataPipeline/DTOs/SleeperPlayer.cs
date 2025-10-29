using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public class SleeperPlayer
{
    [JsonPropertyName("player_id")]
    public required int SleeperId { get; set; }

    [JsonPropertyName("first_name")]
    public required string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public required string LastName { get; set; }

    [JsonPropertyName("search_full_name")]
    public required string MergeName { get; set; }

    [JsonPropertyName("fantasy_positions")]
    public string[]? Position { get; set; }

    [JsonPropertyName("team")]
    public string? Team { get; set; }
}





