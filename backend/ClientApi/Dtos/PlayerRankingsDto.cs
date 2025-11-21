using Shared.Consts;

namespace ClientApi.Dtos;

public record PlayerRankingDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required TeamAbbr Team { get; set; }
    public required IncludedPosition[] Positions { get; set; }
    public required List<PlayerValueDto> Values { get; set; }
    public required DateTime LastUpdated { get; set; }
}