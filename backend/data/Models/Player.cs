using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
  public class Player
  {
    public required string PlayerId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Team { get; set; }
    public string? Position { get; set; }
    public string[]? FantasyPositions { get; set; }
    public string? Status { get; set; }
    public string? InjuryStatus { get; set; }
    public string? SearchFullName { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
  }
  
  public class PlayerStaging
  {
    [Key]
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

    [JsonPropertyName("search_full_name")]
    public string? SearchFullName { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
  }
}


