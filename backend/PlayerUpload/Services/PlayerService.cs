using System.Net.Http.Json;
using System.Text.Json;
using Data.Models;

namespace PlayerUpload.Services;

public sealed class PlayerService
{
    private readonly HttpClient _client;

    public PlayerService(HttpClient client)
    {
        _client = client;
    }

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
            Console.WriteLine($"PlayerService error:{e}");
            return null;
        }
    }

    public void WritePlayersJson(Dictionary<string, PlayerStaging>? players, string filePath)
    {
        if (players == null)
        {
            throw new Exception("players dict is null");
        }

        string jsonStr = JsonSerializer.Serialize(players);
        File.WriteAllText(filePath, jsonStr);

        Console.WriteLine("saved players.json");
    }

}