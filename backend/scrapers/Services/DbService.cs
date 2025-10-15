using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data;
using Data.Models;
using Scrapers.Models;

namespace Scrapers.Services;

public static class DbService
{
    private static readonly string _connectionString;
    static DbService()
    {
        //user-secrets init
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        _connectionString = config["ConnectionStrings:AppDbContext"]
            ?? throw new Exception("no connection string found");
    }

    public static void ProcessData()
    {
        string[] files = GetJsonFiles();
        foreach (var file in files)
        {
            List<ScrapedPlayer> playerValues = DeserializeJson(file);
            SaveToDb(playerValues);
        }
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private static string[] GetJsonFiles()
    {
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "data");
        if (!Directory.Exists(dir))
        {
            throw new Exception($"{dir} not found");
        }

        string[] files = Directory.GetFiles(dir);

        if (files.Length == 0)
        {
            throw new Exception($"{dir} directory returned 0 files");
        }

        return files;
    }

    private static List<ScrapedPlayer> DeserializeJson(string filePath)
    {
        var jsonStr = File.ReadAllText(filePath);
        var playerValues = JsonSerializer.Deserialize<List<ScrapedPlayer>>(jsonStr);

        if (playerValues.Count == 0)
        {
            throw new Exception($"file {filePath} returned 0 player values");
        }
        return playerValues;
    }

    private static void SaveToDb(List<ScrapedPlayer> scraped)
    {
        //todo
        //check for existing player values for the source/player
        //if existing calculate delta and update value,delta,date
        //if not existing insert new record

        //relate player values to player table
        //if sleeperid, use that
        //else use team, position, and searchfullname to match

        using var ctx = CreateContext();
        var players = ctx.Players.ToList();

        foreach (var record in scraped)
        {
            if (record.DataSource == "fc")
            {
                var matched = players.Where(p => p.PlayerId == record.SleeperId);
                
                foreach (var p in matched)
                {
                    Console.WriteLine(p.SearchFullName);
                }
                Console.WriteLine("==============");
            
                Console.WriteLine(record.SearchFullName);
                Console.WriteLine(record.SleeperId);
                Console.WriteLine("---------------");
            }

        }




    }
}