using HtmlAgilityPack;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using WebScraping.Models;

namespace WebScraping.Scrapers;

public abstract class Scraper
{
    public abstract Task ScrapeAndSaveAsync();
    private readonly HtmlWeb _web;
    private readonly HttpClient _client;
    private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
    
    public Scraper()
    {
        //html agility pack
        _web = new HtmlWeb();
        _web.UserAgent = USER_AGENT;

        //http
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    protected HtmlDocument LoadHtml(string url)
    {
        return _web.Load(url);
    }

    protected async Task<T> GetJsonAsync<T>(string url)
    {
        return await _client.GetFromJsonAsync<T>(url);
    }

    protected static async Task SaveToFileAsync(string fileName, List<ScrapedPlayer> data)
    {

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"data/{fileName}.json");
        await using FileStream createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(createStream, data);

        Console.WriteLine($"saved {data.Count} players to {fileName}.json");
    }

    protected static string NormalizeString(string str, bool toLower = false)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        //if encoded html
        str = System.Net.WebUtility.HtmlDecode(str);

        //alphanumeric
        Regex rgx = new Regex("[^a-zA-Z]");
        str = rgx.Replace(str, "");

        if (toLower)
        {
            str = str.ToLower();
        }

        return str;
    } 
}
