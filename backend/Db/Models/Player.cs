using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Db.Models
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
    public DateTime LastUpdated { get; set; }
  }


}


