using System.Text.Json.Serialization;

namespace DataPipeline.DTOs;

public class KtcScrapedPlayer
{
    [JsonPropertyName("playerID")]
    public int KtcId { get; set; }

    [JsonPropertyName("playerName")]
    public required string PlayerName { get; set; }

    [JsonPropertyName("oneQBValues")]
    public required KtcValueData OneQbValues { get; set; }

    [JsonPropertyName("superflexValues")]
    public required KtcValueData SuperFlexValues { get; set; }
}

public class KtcValueData
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("positionalRank")]
    public int PositionRank { get; set; }
}