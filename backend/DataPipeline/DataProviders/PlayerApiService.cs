using System.Net.Http.Json;
using Data.Models;

namespace PlayerUpload;

public sealed class PlayersApiService(HttpClient client)
{
    private readonly HttpClient _client = client;

    public async Task<Dictionary<string, PlayerStaging>?> FetchPlayersAsync()
    {
        try
        {
            var players = await _client.GetFromJsonAsync<Dictionary<string, PlayerStaging>>("players/nfl");
            Console.WriteLine("fetched players");
            return players;
        }
        catch (Exception e)
        {
            Console.WriteLine($"PlayersApiService error:{e}");
            return null;
        }
    }
}