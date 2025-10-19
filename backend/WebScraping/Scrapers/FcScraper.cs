using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Shared.Models;
using WebScraping.Helpers;

namespace WebScraping.Scrapers;

public class FcScraper(HttpClient client) : IScraper
{
    private readonly HttpClient _client = client;

    private class FcResponse
    {
        [JsonPropertyName("player")]
        public required Player Player { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
    private class Player
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("sleeperId")]
        public string? SleeperId { get; set; }

        [JsonPropertyName("maybeTeam")]
        public string? Team { get; set; }
        
        [JsonPropertyName("position")]
        public string? Position { get; set; }
    }

    public async Task<List<ScrapedPlayer>> ScrapeAsync()
    {
        // params
        var isDynasty = true;
        var numQbs = 2;   //i.e. superflex
        var numTeams = 10;
        var ppr = .5;

        string url = $"{Consts.FcBaseUrl}?isDynasty={isDynasty}&numQbs={numQbs}&numTeams={numTeams}&ppr={ppr}&includeAdp=false";

        //fetch from fc
        var fcData = await _client.GetFromJsonAsync<List<FcResponse>>(url) 
            ?? throw new Exception("fc api returned null");

        //convert to type ScrapedPlayer
        var playerData = fcData
            .Where(p => p.Player.Position != Consts.NonPlayerPosition)
            .Select(p => new ScrapedPlayer
            {
                SearchFullName = NormalizeField.Name(p.Player.Name),
                SleeperId      = p.Player.SleeperId,
                Value          = p.Value,
                DataSource     = Consts.SourceFc,
                Position       = p.Player.Position,
                Team           = p.Player.Team,
            }).ToList();

        Console.WriteLine($"fetched {playerData.Count} players");
        return playerData;
    }
}
