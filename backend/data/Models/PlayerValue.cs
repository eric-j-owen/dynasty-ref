namespace Data.Models
{
    public class PlayerValue
    {
        public int Id { get; set; }
        public required string PlayerId { get; set; } //fk
        public required Player Player { get; set; }
        public required string DataSource { get; set; } //ktc
        public required bool IsSuperFlex { get; set; }
        public required string PprFormat { get; set; }
        public required int Value { get; set; }
        public required int ValueDelta { get; set; }
        public required DateTime ScrapedAt { get; set; }
    }
    
    public class ScrapedPlayer
    {
        public required string SearchFullName { get; set; }
        public required int Value { get; set; }
        public bool IsSuperFlex { get; set; } = true; //default to true for now
        public string PprFormat { get; set; } = "0.5"; //defaulting to .5 for now, may include other formats later
        public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
    }
}