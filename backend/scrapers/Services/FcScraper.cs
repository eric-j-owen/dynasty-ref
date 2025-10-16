using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Scrapers.Models;
namespace Scrapers.Services;

public class FcScraper : Scraper
{
    private class FcResponse
    {
        [JsonPropertyName("player")]
        public Player Player { get; set; } = new Player();

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
    private class Player
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sleeperId")]
        public string SleeperId { get; set; }

        [JsonPropertyName("maybeTeam")]
        public string Team { get; set; }
        
        [JsonPropertyName("position")]
        public string Position { get; set; }
    }

    public override async Task ScrapeAndSaveAsync()
    {
        var playerData = await FetchPlayersAsync();
        await SaveToFileAsync("fc", playerData);
    }

    private async Task<List<ScrapedPlayer>> FetchPlayersAsync()
    {
        try
        {
            // params
            var isDynasty = true;
            var numQbs = 2;   //i.e. superflex
            var numTeams = 10;
            var ppr = .5;

            string url = $"https://api.fantasycalc.com/values/current?isDynasty={isDynasty}&numQbs={numQbs}&numTeams={numTeams}&ppr={ppr}&includeAdp=false";

            //fetch from fc
            var fcData = await client.GetFromJsonAsync<List<FcResponse>>(url);
            if (fcData == null)
            {
                throw new Exception("missing fc data");
            }

            //convert to type ScrapedPlayer
            var playerData = fcData
                .Where(p => p.Player.Position != "PICK")
                .Select(p => new ScrapedPlayer
                {
                    SearchFullName = NormalizeString(p.Player.Name),
                    SleeperId      = p.Player.SleeperId,
                    Value          = p.Value,
                    DataSource     = "fc",
                    Position       = p.Player.Position,
                    Team           = p.Player.Team,
                }).ToList();

            Console.WriteLine($"fetched {playerData.Count} players");
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine($"error in fc fetch: {e}");
            throw;
        }
    }
}
