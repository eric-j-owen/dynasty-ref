namespace Data.Models;
public class Player
{
  public required string PlayerId { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public string? FantasyPositions { get; set; }
  public required string Status { get; set; }
  public string? DepthChartPos { get; set; }
  public string? Team { get; set; }
  public string? InjuryStatus { get; set; }
}