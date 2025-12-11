using Shared.Consts;

namespace ClientApi.Dtos.Players;

public record PlayerBaseDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required IncludedPosition[] Positions { get; set; }
}