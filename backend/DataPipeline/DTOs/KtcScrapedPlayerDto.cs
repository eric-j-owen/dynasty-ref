using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public record KtcScrapedPlayerDto
{
    [JsonPropertyName("playerID")]
    public int KtcId { get; set; }

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