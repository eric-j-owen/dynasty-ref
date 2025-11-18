using Shared.Consts;

namespace DataPipeline.DTOs;

public record PlayerValueWithLookupDto
{
    public required Dictionary<DataSource, string> LookupIds { get; set; }
    public required DataSource ValueSource { get; set; }
    public int? OneQbValue { get; set; }
    public int? SuperFlexValue { get; set; }
}