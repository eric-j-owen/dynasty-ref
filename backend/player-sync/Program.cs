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
    Console.WriteLine("missing argument: --fetch or --upsert");
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
        Console.WriteLine("invalid argument: --fetch or --upsert");
    }

}

/*
----------------
methods
----------------
*/
static async Task<List<Player>> FetchPlayersAsync(HttpClient client)
{
    var players = await client.GetFromJsonAsync<List<Player>>("https://api.sleeper.app/v1/league/1185731824680087552/rosters");
    
    return players ?? new();
}

static async Task WritePlayersJsonAsync(List<Player> players)
{
    string fileName = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
    await using FileStream createStream = File.Create(fileName);
    await JsonSerializer.SerializeAsync(createStream, players);

    Console.WriteLine("saved player data");
}

static async Task LoadAndSaveToDbAsync() { }