using HtmlAgilityPack;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Shared.Consts;
using System.Text.Json;
using System.Net;



namespace DataPipeline.DataProviders;

public class KtcScraper(HttpClient client) : IDataProvider<KtcScrapedPlayer>
{
    private readonly HttpClient _client = client;
    private readonly string _endpoint = "/dynasty-rankings?page=0";

    public async Task<List<KtcScrapedPlayer>> ExtractDataAsync()
    {

        var res = await _client.GetStringAsync(_endpoint);

        var html = new HtmlDocument();
        html.LoadHtml(res);

        return ParsePlayersFromDocument(html);
    }

    public static List<KtcScrapedPlayer> ParsePlayersFromDocument(HtmlDocument html)
    {
        const string playersArrayDeclarationStr = "var playersArray = ";

        var scriptNodes = html.DocumentNode.SelectNodes("//script");

        string strNode = "";
        foreach (var node in scriptNodes)
        {
            strNode = node.InnerText;
            if (strNode.Contains(playersArrayDeclarationStr))
            {
                break;
            }
        }

        if (string.IsNullOrEmpty(strNode) || !strNode.Contains(playersArrayDeclarationStr))
        {
            throw new Exception("Ktc scraper could not find players array in script tag");
        }

        var playersArraySplit = strNode.Split(playersArrayDeclarationStr);
        var endOfJsonSplit = playersArraySplit[1].Split(";");
        var json = endOfJsonSplit[0];

        var ktcPlayers = JsonSerializer.Deserialize<List<KtcScrapedPlayer>>(json);

        if (ktcPlayers == null || ktcPlayers.Count == 0)
        {
            throw new Exception("Ktc scraped data is null or empty");
        }

        return ktcPlayers;
    }
}