using System.Text.Json.Serialization;


namespace DataPipeline.DTOs
{
    public class FcPlayerApiResponse
    {
        [JsonPropertyName("player")]
        public required PlayerObj Player { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
    public class PlayerObj
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("sleeperId")]
        public required string SleeperId { get; set; }

        [JsonPropertyName("maybeTeam")]
        public string? Team { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }
    }

}
