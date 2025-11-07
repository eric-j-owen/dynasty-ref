using CsvHelper.Configuration.Attributes;

namespace DataPipeline.DTOs;


public record DynastyProcessIdsDto
{
    [Name("sleeper_id")]
    public int SleeperId { get; set; }

    [Name("ktc_id")]
    public int KtcId { get; set; }
}