using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;

namespace Db.Models;

[Table("player_id_lookup")]
[PrimaryKey(nameof(DataSource), nameof(SourceId))]
public class PlayerIdLookup
{
    [Column("data_source")]
    public required DataSources DataSource { get; set; }

    [Column("source_id")]
    public required string SourceId { get; set; }


    [Column("player_id")]
    public int PlayerId { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public required Player Player { get; set; }
}