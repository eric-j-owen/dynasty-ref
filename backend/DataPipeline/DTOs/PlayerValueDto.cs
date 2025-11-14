using Shared.Consts;

namespace DataPipeline.DTOs;

public record PlayerValueDro
{
    public required Dictionary<DataSource, string> LookupIds { get; set; }
    public required DataSource ValueSource { get; set; }
    public required string PprFormat { get; set; }
    public int? OneQbValue { get; set; }
    public int? SuperFlexValue { get; set; }
}