using System.ComponentModel.DataAnnotations.Schema;
using Shared.Consts;

namespace Db.Models;

[Table("players")]
public class PlayerModel
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
  public required TeamAbbr Team { get; set; }

  [Column("positions")]
  public required IncludedPosition[] Positions { get; set; }

  [Column("last_updated")]
  public required DateTime LastUpdated { get; set; } = DateTime.UtcNow;


  private readonly List<ExternalIdModel> _externalIds = [];
  public IEnumerable<ExternalIdModel> ExternalIds => _externalIds;
  public void AddExternalId(ExternalIdModel id) => _externalIds.Add(id);
}