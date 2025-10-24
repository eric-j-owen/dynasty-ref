using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Models;

[Table("incomplete_data_players")]
public class IncompleteDataPlayers
{
    [Column("id")]
    public int Id { get; set; }

    [Column("raw_data", TypeName = "jsonb")]
    public required string RawData { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("is_resolved")]
    public bool IsResolved { get; set; } = false;

    [Column("created_at")]
    public DateTime CreateddAt { get; set; } = DateTime.UtcNow;

}