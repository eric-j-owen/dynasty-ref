using HtmlAgilityPack;
using System.Text.Json;
using Data.Models;

namespace Scrapers.Services;
public class Scraper
{
    protected HtmlWeb web;

    public Scraper()
    {
        web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
    }

    public async Task SaveToFileAsync(string fileName, List<ScrapedPlayer> data)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"data/{fileName}.json");
            await using FileStream createStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(createStream, data);

            Console.WriteLine($"saved ${data.Count} players to {fileName}.json");
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e}");
            throw;
        }
    }
}
