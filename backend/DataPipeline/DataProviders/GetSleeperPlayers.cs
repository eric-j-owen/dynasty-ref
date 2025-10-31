using System.Net.Http.Json;
using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataPipeline.DataProviders;

public class GetSleeperPlayers(HttpClient client, ILogger<GetSleeperPlayers> logger) : IDataProvider<SleeperPlayer>
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

            logger.LogInformation("success: fetch players sleeper");
            return [.. json.Values];
        }

        catch (Exception e)
        {
            logger.LogError("error fetcching sleeper players: {e}", e);
            throw;
        }
    }
}