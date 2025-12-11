using Shared.Consts;

namespace ClientApi.Dtos.Players;

public record PlayerDetailsDto : PlayerBaseDto
{
    public required TeamAbbr Team { get; set; }
    public int? Age { get; set; }
    public string? InjuryStatus { get; set; }
    public string? College { get; set; }

}