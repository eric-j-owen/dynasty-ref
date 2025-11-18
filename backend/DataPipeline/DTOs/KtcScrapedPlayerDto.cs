using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public record KtcScrapedPlayerDto
{
    [JsonPropertyName("playerID")]
    public required int KtcId { get; set; }

    [JsonPropertyName("mflid")]
    public int? MflId { get; set; }

    [JsonPropertyName("oneQBValues")]
    public required KtcValueData OneQbValues { get; set; }

    [JsonPropertyName("superflexValues")]
    public required KtcValueData SuperFlexValues { get; set; }

}

public record KtcValueData
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}