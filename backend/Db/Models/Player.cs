using System.ComponentModel.DataAnnotations.Schema;
using Shared.Consts;

namespace Db.Models;

[Table("players")]
public class Player
{
  [Column("id")]
  public int Id { get; set; }

  [Column("merged_full_name")]
  public required string MergeFullName { get; set; }

  [Column("first_name")]
  public required string FirstName { get; set; }

  [Column("last_name")]
  public required string LastName { get; set; }

  [Column("team")]
  public Teams? Team { get; set; }

  [Column("positions")]
  public required Positions[] Positions { get; set; }

  [Column("last_updated")]
  public required DateTime LastUpdated { get; set; }
}