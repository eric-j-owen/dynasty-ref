using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models; 
using System.Text.Json;
using System.IO;

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


//read and save data local
var players = await ProcessPlayersAsync(client);
string fileName = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
await using FileStream createStream = File.Create(fileName);
await JsonSerializer.SerializeAsync(createStream, players);


static async Task<List<Player>> ProcessPlayersAsync(HttpClient client)
{
    var players = await client.GetFromJsonAsync<List<Player>>("https://api.sleeper.app/v1/league/1185731824680087552/rosters");

    
    return players ?? new();
}
