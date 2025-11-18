using System.Net.Http.Json;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;

namespace DataPipeline.DataPipeline.Extract;

public class SleeperPlayersExtract(HttpClient client) : IDataProvider<SleeperPlayerDto>
{
    private readonly HttpClient _client = client;

    private readonly string _endpoint = "players/nfl";

    public async Task<List<SleeperPlayerDto>> ExtractDataAsync()
    {
        try
        {
            var json = await _client.GetFromJsonAsync<Dictionary<string, SleeperPlayerDto>>(_endpoint);
            if (json == null || json.Count == 0)
            {
                throw new Exception("ExtractSleeperPlayers: players is empty or null");
            }

            return [.. json.Values];
        }

        catch (Exception e)
        {
            Console.WriteLine($"error data fetch: {e}");
            throw;
        }
    }
}