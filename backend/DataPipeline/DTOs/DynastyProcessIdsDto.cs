using CsvHelper.Configuration.Attributes;

namespace DataPipeline.DTOs;


public record DynastyProcessIdsDto
{
    [Name("sleeper_id")]
    public string? SleeperId { get; set; }

    [Name("ktc_id")]
    public string? KtcId { get; set; }

    [Name("mfl_id")]
    public string? MflId { get; set; }
}