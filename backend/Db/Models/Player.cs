using System.ComponentModel.DataAnnotations.Schema;
using Shared.Consts;

namespace Db.Models;

[Table("players")]
public class Player
{
  [Column("id")]
  public int Id { get; set; }

  [Column("normalized_name")]
  public required string NormalizedName { get; set; }

  [Column("first_name")]
  public required string FirstName { get; set; }

  [Column("last_name")]
  public required string LastName { get; set; }

  [Column("team")]
  public PlayerConsts.Team? Team { get; set; }

  [Column("positions")]
  public required PlayerConsts.IncludedPosition[] Positions { get; set; }

  [Column("last_updated")]
  public DateTime LastUpdated { get; set; } = DateTime.UtcNow;


  private readonly List<ExternalIdPlayerLookup> _externalIds = [];
  public IEnumerable<ExternalIdPlayerLookup> ExternalIds => _externalIds;
  public void AddExternalId(ExternalIdPlayerLookup id) => _externalIds.Add(id);
}