using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;
namespace Db.Models;


[Table("player_values")]
[Index(nameof(DataSource), nameof(PprFormat), nameof(IsSuperFlex), nameof(PlayerId), nameof(CreatedAt), IsUnique = true)]
public class PlayerValueModel
{
    [Column("id")]
    public int Id { get; set; }

    [Column("data_source")]
    public required DataSource DataSource { get; set; }

    [Column("is_super_flex")]
    public required bool IsSuperFlex { get; set; }

    [Column("ppr_format")]
    public required string PprFormat { get; set; } = "0.5";

    [Column("value")]
    public required int Value { get; set; }

    [Column("created_at")]
    public required DateOnly CreatedAt { get; set; }



    [Column("player_id")]
    public int PlayerId { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public required PlayerModel Player { get; set; }
}
