using Shared.Consts;

namespace ClientApi.Dtos.Players;

public record PlayerValueDto
{
    public bool IsSuperFlex { get; set; }
    public DataSource Source { get; set; }
    public int Value { get; set; }

}