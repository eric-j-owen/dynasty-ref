using Shared.Consts;

namespace ClientApi.Dtos;

public record ExternalIdDto
{
    public DataSource Source { get; set; }
    public required string SourceId { get; set; }
}