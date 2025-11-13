using Shared.Consts;

namespace DataPipeline.DTOs;

// used for externalid transformer, includes sleeper id to more easily add player relation during loading
public record ExternalIdWithLookupDto
{
    public required string SleeperId;
    public required DataSource DataSource;
    public required string SourceId;
}