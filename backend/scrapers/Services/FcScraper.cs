using System.Net.Http.Json;
using System.Text.Json.Serialization;

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
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("sleeperId")]
        public string SleeperId { get; set; } = string.Empty;
    }

    public async Task<List<ScrapedPlayer>> FetchPlayersAsync()
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
            var playerData = fcData.Select(p => new ScrapedPlayer
            {
                SearchFullName = NormalizeName(p.Player.Name),
                SleeperId = p.Player.SleeperId,
                Value = p.Value,
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

    public override async Task ScrapeAndSaveAsync()
    {
        var playerData = await FetchPlayersAsync();
        await SaveToFileAsync("fc-rankings", playerData);
    }
}
