using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Data;
using Data.Models;
using Data.Services;
using PlayerUpload;
using Shared.Services;

//secrets config
IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

//services
builder.Services.AddHttpClient<PlayersApiService>(
    client =>
    {
        client.BaseAddress = new Uri("https://api.sleeper.app/v1/");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
            );
    }
);
builder.Services.AddTransient<PlayerDbService>();
builder.Services.AddTransient<FileService>();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config.GetConnectionString("AppDbContext")));

using IHost host = builder.Build();

//control flow
try
{
    if (args.Length == 0)
    {
        throw new Exception("missing argument: --push --pull");
    }
    else
    {
        const string FILE_NAME= "players-master";

        var playersApiService = host.Services.GetRequiredService<PlayersApiService>();
        var dbService = host.Services.GetRequiredService<PlayerDbService>();
        var fileService = host.Services.GetRequiredService<FileService>();

        //fetch all players and save locally
        if (args.Contains("--pull"))
        {
            var players = await playersApiService.FetchPlayersAsync();
            fileService.WriteToFileJson(FILE_NAME, players);
        }

        //update db with players data
        else if (args.Contains("--push"))
        {
            var data = fileService.ReadFromFileJson<Dictionary<string, PlayerStaging>>(FILE_NAME) ?? throw new Exception("player data is null");
            dbService.ProcessPlayersFromFile(data);
        }

        else
        {
            throw new Exception("invalid argument: --push --pull");
        }
    }
}
catch (Exception e)
{
    Console.WriteLine($"error: {e}");
}