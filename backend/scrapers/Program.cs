// using System.Net.Http.Headers;
// using System.Net.Http.Json;
// using Microsoft.Extensions.Configuration;
// using Microsoft.EntityFrameworkCore;
using System.Text.Json;
// using System.IO;
using HtmlAgilityPack;

using Data;
using Data.Models;


await Scraper();
async Task Scraper()
{
    //initialize
    var web = new HtmlWeb();
    web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    int page = 0;
    int limit = 9; //hard coded for now
    var playerData = new List<ScrapedPlayer>();


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

    //save local 
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "data/ktc-rankings.json");
    await using FileStream createStream = File.Create(filePath);
    await JsonSerializer.SerializeAsync(createStream, playerData);

}
