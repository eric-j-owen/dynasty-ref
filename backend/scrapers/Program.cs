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
    //initialize and set user agent
    var web = new HtmlWeb();
    web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    //testing- load page 1 of ktc rankings
    var html = web.Load("https://keeptradecut.com/dynasty-rankings?page=0");



    var playerData = new List<ScrapedPlayer>();

    //player div
    var htmlElements = html.DocumentNode.QuerySelectorAll("div.onePlayer");

    //extract player data
    foreach (var el in htmlElements)
    {
        var name = el.QuerySelector("div.player-name a").InnerText;
        var valueTxt = el.QuerySelector("div.value").InnerText;
        int value = int.Parse(valueTxt);

        var player = new ScrapedPlayer()
        {
            SearchFullName = name,
            Value = value
        };

        playerData.Add(player);

    }
    
    //save local 
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "data/test.json");
    await using FileStream createStream = File.Create(filePath);
    await JsonSerializer.SerializeAsync(createStream, playerData);

}
