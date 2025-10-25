using HtmlAgilityPack;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Shared.Consts;
using System.Text.Json;
using System.Net;



namespace DataPipeline.DataProviders;

public class KtcProvider : IDataProvider<KtcScrapedPlayer>
{
    private readonly HtmlWeb _web;
    private readonly string _endpoint;

    public KtcProvider()
    {
        _web = new HtmlWeb { UserAgent = Api.UserAgent };
        _endpoint = $"{Api.KtcBaseUrl}/dynasty-rankings?page=0";
    }

    public Task<List<KtcScrapedPlayer>> ExtractDataAsync()
    {

        var html = _web.Load(_endpoint);
        var ktcPlayers = ParsePlayersFromDocument(html);

        return Task.FromResult(ktcPlayers);
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