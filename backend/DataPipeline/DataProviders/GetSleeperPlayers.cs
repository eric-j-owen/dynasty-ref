using System.Net.Http.Json;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;

namespace DataPipeline.DataProviders;

public class GetSleeperPlayers(HttpClient client) : IDataProvider<SleeperPlayer>
{
    private readonly HttpClient _client = client;
    private readonly string _endpoint = "players/nfl";

    public async Task<List<SleeperPlayer>> ExtractDataAsync()
    {
        try
        {
            var json = await _client.GetFromJsonAsync<Dictionary<string, SleeperPlayer>>(_endpoint);
            if (json == null || json.Count == 0)
            {
                throw new Exception("ExtractSleeperPlayers: players is empty or null");
            }

            return [.. json.Values];
        }

        catch (Exception e)
        {
            Console.WriteLine($"ExtractSleeperPlayers error: {e}");
            throw;
        }
    }
}