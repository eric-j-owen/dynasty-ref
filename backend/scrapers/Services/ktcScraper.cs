using Data.Models;

namespace Scrapers.Services;

public class KtcScraper : Scraper
{
    public async Task<List<ScrapedPlayer>> ScrapeAsync()
    {
        int page = 0;
        int limit = 1;//hard coded for now
        List<ScrapedPlayer> playerData = new();

        while (page <= limit)
        {
            try
            {
                //extract player elements for current page
                var html = web.Load($"https://keeptradecut.com/dynasty-rankings?page={page}");
                var htmlElements = html.DocumentNode.QuerySelectorAll("div.onePlayer");

                Console.WriteLine($"page: {page} loaded. {htmlElements.Count} player elements.");

                //parse current pages elements and add to player list
                foreach (var el in htmlElements)
                {
                    var name = el.QuerySelector("div.player-name a").InnerText;
                    var valueTxt = el.QuerySelector("div.value").InnerText;
                    int value = int.Parse(valueTxt);

                    var player = new ScrapedPlayer() //using default values for fields superflex and scoringformat
                    {
                        SearchFullName = name,
                        Value = value
                    };

                    playerData.Add(player);
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

    public async Task ScrapeAndSaveAsync()
    {
        var playerData = await ScrapeAsync();
        await SaveToFileAsync("ktc-rankings", playerData);
    }
}