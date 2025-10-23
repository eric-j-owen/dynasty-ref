using HtmlAgilityPack;
using DataPipeline.Helpers;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
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

    protected virtual HtmlDocument LoadHtml(string path)
    {
        return _web.Load(path);
    }

    /*
        gets script tags
        extracts contents of the script tag that contains playersArray
        converts script tag to a string, 
        splits it on the variable playersArray
        splits again at json structure ending
        deserializes and returns
    */
    public Task<List<KtcScrapedPlayer>> ExtractDataAsync(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("ktcprovider: missing path arg");
        }

        const string playersArrayDeclarationStr = "var playersArray = ";

        var html = LoadHtml(path);
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


        return Task.FromResult(ktcPlayers);
    }

}