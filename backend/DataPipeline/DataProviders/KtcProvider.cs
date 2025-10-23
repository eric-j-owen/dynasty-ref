using HtmlAgilityPack;
using DataPipeline.Helpers;
using DataPipeline.DTOs;
using System.Text.Json;

namespace DataPipeline.DataProviders;

public class KtcProvider : IDataProvider<KtcScrapedPlayer>
{
    private readonly HtmlWeb _web;

    public KtcProvider()
    {
        _web = new HtmlWeb
        {
            UserAgent = Consts.UserAgent
        };
    }

    public string DataSource
    {
        get { return Consts.DataSources.Ktc; }
    }

    /*
        gets script tags
        extracts contents of the script tag that contains playersArray
        converts script tag to a string, 
        splits it on the variable playersArray
        splits again at json structure ending
        deserializes and returns
    */
    public List<KtcScrapedPlayer> ExtractData()
    {
        const string playersArrayDeclarationStr = "var playersArray = ";

        var html = _web.Load($"{Consts.Paths.KtcBase}?page=0");
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