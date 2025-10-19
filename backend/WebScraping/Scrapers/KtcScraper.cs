using Shared.Models;
using Fizzler.Systems.HtmlAgilityPack;
using WebScraping.Helpers;
using HtmlAgilityPack;

namespace WebScraping.Scrapers;
public class KtcScraper : IScraper
{
    private readonly HtmlWeb _web;

    public KtcScraper()
    {
        _web = new HtmlWeb();
        _web.UserAgent = Consts.UserAgent;
    }


    public async Task<List<ScrapedPlayer>> ScrapeAsync()
    {
        int page = 0;
        List<ScrapedPlayer> playerData = [];

        while (true)
        {
            //extract player elements for current page
            var html = _web.Load($"{Consts.KtcBaseUrl} page={page}");
            var htmlElements = html.DocumentNode.QuerySelectorAll("div.onePlayer");

            if (!htmlElements.Any())
            {
                Console.WriteLine("no more players found");
                break;
            }

            Console.WriteLine($"page: {page} loaded. {htmlElements.Count()} player elements.");

            //parse current pages elements and add to player list
            foreach (var el in htmlElements)
            {
                var name = el.QuerySelector("div.player-name a").InnerText;
                var valueTxt = el.QuerySelector("div.value").InnerText;
                int value = int.Parse(valueTxt);
                var team = el.QuerySelector("span.player-team").InnerText;
                var position = el.QuerySelector("p.position").InnerText;

                if (position != Consts.NonPlayerPosition)
                {
                    playerData.Add(ParsePlayer(name, value, team, position));
                }
            }

            await Task.Delay(2000);
            page++;
        }

        return playerData;
    }

    public static ScrapedPlayer ParsePlayer(string name, int value, string team, string position, string dataSource = Consts.SourceKtc)
    {
        return new ScrapedPlayer(){
            SearchFullName = NormalizeField.Name(name),
            Value = value,
            Team = MapTeam(team),
            Position = NormalizeField.Position(position),
            DataSource = dataSource
        };
    }
    
    // map ktc teams names to values used in sleeper api
    private static string? MapTeam(string team)
    {
        var teamMappings = new Dictionary<string, string?> // <ktc, sleeper>
        {
            {"SFO", "SF"},
            {"NOS", "NO"},
            {"JAC", "JAX"},
            {"GBP", "GB"},
            {"KCC", "KC"},
            {"NEP", "NE"},
            {"TBB", "TB"},
            {"LVR", "LV"},
            {"FA", null }
        };

        if (teamMappings.TryGetValue(team, out string? value))
        {
            return value;
        }

        return team;
    }
    
}