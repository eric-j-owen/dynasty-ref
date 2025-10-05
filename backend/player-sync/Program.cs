using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models; 



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


await ProcessRepositoriesAsync(client);


static async Task ProcessRepositoriesAsync(HttpClient client)
{
    var json = await client.GetStringAsync("https://api.sleeper.app/v1/user/837121062331318272/leagues/nfl/2025");

    Console.Write(json);
}
