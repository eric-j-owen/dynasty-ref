namespace WebScraping.Models;

public class ScrapedPlayer
{
    public required int Value { get; set; }
    public required string DataSource { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;

    //fantasy options
    public bool IsSuperFlex { get; set; } = true; //default to true for now
    public string PprFormat { get; set; } = "0.5"; //defaulting to .5 for now, may include other formats later

    //values to use for mapping to players table
    public required string SearchFullName { get; set; }
    public string? SleeperId { get; set; }
    public string? Team { get; set; }
    public string? Position { get; set; }
}