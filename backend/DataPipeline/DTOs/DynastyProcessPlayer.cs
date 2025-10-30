using CsvHelper.Configuration.Attributes;

namespace DataPipeline.DTOs;


public class DynastyProcessPlayer
{
    [Name("sleeper_id")]
    public int SleeperId { get; set; }

    [Name("ktc_id")]
    public int KtcId { get; set; }

    [Name("name")]
    public required string Name { get; set; }

    [Name("merge_name")]
    public required string MergeName { get; set; }

    [Name("position")]
    public required string Position { get; set; }

    [Name("team")]
    public required string Team { get; set; }
}