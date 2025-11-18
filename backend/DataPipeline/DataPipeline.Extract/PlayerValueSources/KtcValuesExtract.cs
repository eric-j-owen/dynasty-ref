using HtmlAgilityPack;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using System.Text.Json;
using Shared.Consts;


namespace DataPipeline.DataPipeline.Extract.PlayerValueSources;

public class KtcValuesExtract(HttpClient client) : IDataProvider<PlayerValueWithLookupDto>
{
    private readonly HttpClient _client = client;
    private readonly string _endpoint = "/dynasty-rankings?page=0";

    public async Task<List<PlayerValueWithLookupDto>> ExtractDataAsync()
    {
        try
        {
            var res = await _client.GetStringAsync(_endpoint);

            var html = new HtmlDocument();
            html.LoadHtml(res);

            var data = ParsePlayersFromDocument(html);

            List<PlayerValueWithLookupDto> result = [];
            foreach (var player in data)
            {
                var baseEntity = new PlayerValueWithLookupDto
                {
                    ValueSource = DataSource.KeepTradeCut,
                    LookupIds = new()
                    {
                        [DataSource.KeepTradeCut] = player.KtcId.ToString(),
                        [DataSource.Mfl] = player.MflId.ToString() ?? "",
                    }
                };

                if (player.OneQbValues?.Value != null)
                {
                    result.Add(baseEntity with
                    {
                        OneQbValue = player.OneQbValues.Value
                    });
                }

                if (player.SuperFlexValues?.Value != null)
                {
                    result.Add(baseEntity with
                    {
                        SuperFlexValue = player.SuperFlexValues.Value,
                    });
                }
            }

            return result;

        }
        catch (Exception e)
        {
            Console.WriteLine($"error during ktc scrape: {e}");
            throw;
        }
    }

    public static List<KtcScrapedPlayerDto> ParsePlayersFromDocument(HtmlDocument html)
    {
        try
        {
            const string playersArrayDeclarationStr = "var playersArray = ";

            var scriptNodes = html.DocumentNode.SelectNodes("//script") ?? throw new Exception("scraper: did not find <script> tags");

            string strNode = "";
            foreach (var node in scriptNodes)
            {
                strNode = node.InnerText;

                if (strNode.Contains(playersArrayDeclarationStr))
                {
                    break;
                }
            }

            if (!strNode.Contains(playersArrayDeclarationStr))
            {
                throw new Exception("Ktc scraper could not find data in script tag");
            }

            var playersArraySplit = strNode.Split(playersArrayDeclarationStr);
            var endOfJsonSplit = playersArraySplit[1].Split(";");
            var json = endOfJsonSplit[0];

            var ktcPlayers = JsonSerializer.Deserialize<List<KtcScrapedPlayerDto>>(json);

            if (ktcPlayers == null || ktcPlayers.Count == 0)
            {
                throw new Exception("Ktc scraped data is null or empty");
            }

            return ktcPlayers;

        }
        catch (Exception e)
        {
            Console.WriteLine($"scraper failed: {e}");
            throw;
        }
    }
}