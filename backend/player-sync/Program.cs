using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models; 
using System.Text.Json;
using System.IO;


/*
----------------
configuration
----------------
*/

//secrets config
IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();


//httpclient config
using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));

//db config
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(config["ConnectionStrings:AppDbContext"])
    .Options;

/*
----------------
control flow
----------------
*/
if (args.Length == 0)
{
    throw new ArgumentException("missing argument: --fetch or --upsert");
}
else
{
    //fetch all players and save locally
    if (args.Contains("--fetch"))
    {
        var players = await FetchPlayersAsync(client);
        await WritePlayersJsonAsync(players);
    }

    //update db with players.json
    else if (args.Contains("--upsert"))
    {
        Console.WriteLine("inside upsert");
    }

    else
    {
        throw new ArgumentException("invalid argument: --fetch or --upsert");
    }

}

/*
----------------
methods
----------------
*/


static async Task<Dictionary<string, Player>> FetchPlayersAsync(HttpClient client)
{
    try
    {
        string url = "https://api.sleeper.app/v1/players/nfl";
        var players = await client.GetFromJsonAsync<Dictionary<string, Player>>(url);
   
        Console.WriteLine("fetched players");
        return players ?? new();
    }
    catch (Exception e)
    {
        Console.WriteLine($"error: {e}");
        throw;
    }
   
}

static async Task WritePlayersJsonAsync(Dictionary<string, Player> players)
{
    try
    {
        string fileName = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, players);

        Console.WriteLine("saved players.json");
    }
    catch (Exception e)
    {
        Console.WriteLine($"error: {e}");
        throw;
    }
}

// static async Task LoadAndSaveToDbAsync() { }

