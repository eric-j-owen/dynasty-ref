using HtmlAgilityPack;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Scrapers.Services;

public abstract class Scraper
{
    protected HtmlWeb web;
    protected HttpClient client;
    private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
    public class ScrapedPlayer
    {
        public string? SleeperId { get; set; }
        public required string SearchFullName { get; set; }
        public required int Value { get; set; }
        public bool IsSuperFlex { get; set; } = true; //default to true for now
        public string PprFormat { get; set; } = "0.5"; //defaulting to .5 for now, may include other formats later
        public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
    }

    public Scraper()
    {
        //html agility pack
        web = new HtmlWeb();
        web.UserAgent = USER_AGENT;

        //http
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

    public abstract Task ScrapeAndSaveAsync();

    // need to impliment
    protected string NormalizeName(string name)
    {
        return name;
    } 
}
