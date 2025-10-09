namespace Data.Models;
public class PlayerValue
{
    public int Id { get; set; }
    public required string PlayerId { get; set; } //fk
    public required Player Player { get; set; }
    public required string DataSource { get; set; } //ktc, fc
    public required bool IsSuperFlex { get; set; }
    public required string PprFormat { get; set; }
    public required int Value { get; set; }
    public required int ValueDelta { get; set; }
    public required DateTime ScrapedAt { get; set; }
}
