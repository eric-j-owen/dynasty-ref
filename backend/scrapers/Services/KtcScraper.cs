using Scrapers.Models;

namespace Scrapers.Services;

public class KtcScraper : Scraper
{
    public override async Task ScrapeAndSaveAsync()
    {
        var playerData = await ScrapeAsync();
        await SaveToFileAsync("ktc", playerData);
    }

    // map ktc teams names to values used in sleeper api
    private static string MapTeam(string team)
    {
        var teamMappings = new Dictionary<string, string> // <ktc value, sleeper value>
        {
            {"SFO", "SF"},
            {"NOS", "NO"},
            {"JAC", "JAX"},
            {"GBP", "GB"},
            {"KCC", "KC" },
            {"NEP", "NE"},
            {"TBB", "TB" },
            {"LVR", "LV" },
        };

        if (teamMappings.ContainsKey(team))
        {
            team = teamMappings[team];
        }

        return team;
    }

    private async Task<List<ScrapedPlayer>> ScrapeAsync()
    {
        int page = 0;
        List<ScrapedPlayer> playerData = new();

        while (true)
        {
            try
            {
                //extract player elements for current page
                var html = web.Load($"https://keeptradecut.com/dynasty-rankings?page={page}");
                var htmlElements = html.DocumentNode.QuerySelectorAll("div.onePlayer");

                if (htmlElements.Count == 0)
                {
                    Console.WriteLine("no more players found");
                    break;
                }

                Console.WriteLine($"page: {page} loaded. {htmlElements.Count} player elements.");

                //parse current pages elements and add to player list
                foreach (var el in htmlElements)
                {
                    var name = el.QuerySelector("div.player-name a").InnerText;
                    var valueTxt = el.QuerySelector("div.value").InnerText;
                    int value = int.Parse(valueTxt);
                    var team = el.QuerySelector("span.player-team").InnerText;
                    var position = el.QuerySelector("p.position").InnerText;

                    if (position != "PICK") // only save player values
                    {
                        var player = new ScrapedPlayer() //using default values for fields superflex and scoringformat
                        {
                            SearchFullName = NormalizeString(name),
                            Value = value,
                            Team = MapTeam(team),
                            Position = NormalizeString(position, false),
                            DataSource = "ktc"
                        };

                        playerData.Add(player);
                    }

                }

                await Task.Delay(2000);
                page++;
            }

            catch (Exception e)
            {
                Console.WriteLine($"error page {page}: {e}");
            }
        }

        return playerData;
    }
}