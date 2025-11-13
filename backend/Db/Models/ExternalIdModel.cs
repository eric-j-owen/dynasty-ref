using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;

namespace Db.Models;

[Table("external_id_player_lookup")]
[PrimaryKey(nameof(DataSource), nameof(SourceId))]
public class ExternalIdModel
{
    [Column("data_source")]
    public required DataSource DataSource { get; set; }

    [Column("source_id")]
    public required string SourceId { get; set; }


    [Column("player_id")]
    public int PlayerId { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public PlayerModel? Player { get; set; }
}