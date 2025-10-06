using System.Text.Json.Serialization;

namespace Data.Models;


public class Player
{
  [JsonPropertyName("player_id")]
  public required string PlayerId { get; set; }

  [JsonPropertyName("first_name")]
  public required string FirstName { get; set; }

  [JsonPropertyName("last_name")]
  public required string LastName { get; set; }

  [JsonPropertyName("team")]
  public string? Team { get; set; }

  [JsonPropertyName("position")]
  public string? Position{ get; set; }

  [JsonPropertyName("fantasy_positions")]
  public string[]? FantasyPositions { get; set; }

  [JsonPropertyName("status")]
  public string? Status { get; set; }

  [JsonPropertyName("injury_status")]
  public string? InjuryStatus { get; set; }

  [JsonPropertyName("last_updated")]
  public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
