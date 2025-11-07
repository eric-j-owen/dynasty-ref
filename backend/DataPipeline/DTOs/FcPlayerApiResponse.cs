using System.Text.Json.Serialization;


namespace DataPipeline.DTOs
{
    public record FcPlayerApiResponse
    {
        [JsonPropertyName("player")]
        public required PlayerObj Player { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
    public record PlayerObj
    {

        [JsonPropertyName("sleeperId")]
        public required string SleeperId { get; set; }

    }

}
