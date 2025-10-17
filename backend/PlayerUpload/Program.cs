using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Data;
using Data.Services;
using PlayerUpload.Services;

//config
const string FILE_PATH = "../Data/json-data/players-master.json";

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<PlayerService>(
    client =>
    {
        client.BaseAddress = new Uri("https://api.sleeper.app/v1/");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
            );
    }
);
builder.Services.AddTransient<PlayerDbService>();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config.GetConnectionString("AppDbContext")));

using IHost host = builder.Build();

//control flow
try
{
    if (args.Length == 0)
    {
        throw new Exception("missing argument: --push --pull --all");
    }
    else
    {
        var playerService = host.Services.GetRequiredService<PlayerService>();
        var dbService = host.Services.GetRequiredService<PlayerDbService>();

        //fetch all players and save locally
        if (args.Contains("--pull"))
        {
            var players = await playerService.FetchPlayersAsync();
            playerService.WritePlayersJson(players, FILE_PATH);
        }

        //update db with players.json
        else if (args.Contains("--push"))
        {
            dbService.ProcessPlayersFromFile(FILE_PATH);
        }

        //run everything
        else if (args.Contains("--all"))
        {
            var players = await playerService.FetchPlayersAsync();
            playerService.WritePlayersJson(players, FILE_PATH);
            dbService.ProcessPlayersFromFile(FILE_PATH);
        }

        else
        {
            throw new Exception("invalid argument: --push --pull --all");
        }

    }
}
catch (Exception e)
{
    Console.WriteLine($"error: {e}");
}