// using System.Net.Http.Json;
// using DataPipeline.DataProviders;
// using DataPipeline.DTOs;
// using DataPipeline.Helpers;

// namespace WebScraping.Scrapers;

// public class FcScraper(HttpClient client) : IDataProvider
// {
//     private readonly HttpClient _client = client;


//     public async Task<List<KtcScrapedPlayer>> ScrapeAsync()
//     {
//         // params
//         var isDynasty = true;
//         var numQbs = 2;   //i.e. superflex
//         var numTeams = 10;
//         var ppr = .5;

//         string url = $"{Consts.FcBaseUrl}/values/current?isDynasty={isDynasty}&numQbs={numQbs}&numTeams={numTeams}&ppr={ppr}&includeAdp=false";

//         //fetch from fc
//         var fcData = await _client.GetFromJsonAsync<List<FcPlayerApiResponse>>(url)
//             ?? throw new Exception("fc api returned null");

//         //convert to type ScrapedPlayer
//         var playerData = fcData
//             .Where(p => p.Player.Position != Consts.NonPlayerPosition)
//             .Select(p => new KtcScrapedPlayer
//             {
//                 SearchFullName = NormalizeField.Name(p.Player.Name),
//                 SleeperId = p.Player.SleeperId,
//                 Value = p.Value,
//                 DataSource = Consts.SourceFc,
//                 Position = p.Player.Position,
//                 Team = p.Player.Team,
//             }).ToList();

//         Console.WriteLine($"fetched {playerData.Count} players");
//         return playerData;
//     }
// }
