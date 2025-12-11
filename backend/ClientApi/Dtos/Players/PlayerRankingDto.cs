namespace ClientApi.Dtos.Players;

public record PlayerRankingDto : PlayerBaseDto
{
    public required List<PlayerValueDto> Values { get; set; }
    public required DateTime LastUpdated { get; set; }
}