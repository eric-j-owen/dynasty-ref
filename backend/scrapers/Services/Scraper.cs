using HtmlAgilityPack;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Scrapers.Models;
namespace Scrapers.Services;

public abstract class Scraper
{
    public abstract Task ScrapeAndSaveAsync();
    protected HtmlWeb web;
    protected HttpClient client;
    private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
    
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

    protected static async Task SaveToFileAsync(string fileName, List<ScrapedPlayer> data)
    {
       
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"data/{fileName}.json");
        await using FileStream createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(createStream, data);

        Console.WriteLine($"saved {data.Count} players to {fileName}.json");
    }

    protected static string NormalizeString(string? str, bool toLower = true)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        str = System.Net.WebUtility.HtmlDecode(str);
        Regex rgx = new Regex("[^a-zA-Z]");
        str = rgx.Replace(str, "");
        if (toLower)
        {
            str = str.ToLower();
        }
        return str;
    } 
}
