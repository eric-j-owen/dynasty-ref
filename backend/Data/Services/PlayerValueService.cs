using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data.Models;
using Shared.Models;

namespace Data.Services;

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

        if (playerValues?.Count == 0 || playerValues == null)
        {
            throw new Exception($"file {filePath} returned 0 player values");
        }
        return playerValues;
    }

    private static void SaveToDb(List<ScrapedPlayer> scraped)
    {
        /*todo
            []check for existing player values for the source/player
            []if existing calculate delta and update value,delta,date
            []if not existing insert new record
            [x]relate player values to player table
            [x]if sleeperid, use that
            [x]else use team, position, and searchfullname to match
            []refactor
        */

        using var ctx = CreateContext();
        var players = ctx.Players.ToList();
        IEnumerable<Player> matched = Enumerable.Empty<Player>();

        foreach (var record in scraped)
        {
            //match with sleeperid if available
            if (!string.IsNullOrEmpty(record.SleeperId))
            {
                matched = players.Where(p => p.PlayerId == record.SleeperId);
            }

            //no sleeperid, try to match with name and position
            else if (!string.IsNullOrEmpty(record.Position))
            {
                matched = players
                    .Where(p =>
                        MatchNames(p.SearchFullName, record.SearchFullName) &&
                        p.Position == record.Position);


                //if multiple matches try to match teams
                if (matched.Count() > 1)
                {
                    var teamMatched = matched.Where(p => p.Team == record.Team);

                    //match found by team
                    if (teamMatched.Count() == 1)
                    {
                        matched = teamMatched;
                        Console.WriteLine($"resolved dup for {record.SearchFullName}");
                    }

                    //unable to resolve duplicates
                    else
                    {
                        Console.WriteLine($"unresolved duplicates for: {record.SearchFullName}");
                    }
                }
            }

            else
            {
                Console.WriteLine($"incomplete data for {record.SearchFullName}");
            }

            if (!matched.Any())
            {
                Console.WriteLine($"no match found for {record.SearchFullName}");
                Console.WriteLine(record.DataSource);
            }

        }

    }

    private static bool MatchNames(string? name1, string? name2)
    {
        if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
        {
            return false;
        }

        if (name1 == name2)
        {
            return true;
        }

        // account for nicknames being used in different data sources
        var nameMappings = new Dictionary<string, string>
        {
            {"zonovanknight", "bamknight" },
            {"marquisebrown", "hollywoodbrown" },
            {"chigoziemokonkwo", "chigokonkwo" },
            {"gabrieldavis", "gabedavis" },
        };

        string mappedName1 = nameMappings.ContainsKey(name1) ? nameMappings[name1] : name1;
        string mappedName2 = nameMappings.ContainsKey(name2) ? nameMappings[name2] : name2;

        if (mappedName1 == mappedName2)
        {
            return true;
        }

        //check without suffixes
        if (RemoveSuffix(name1) == RemoveSuffix(name2))
        {
            return true;
        }

        return false;
    }

    //normalize inconsistent suffixes being included/excluded between data sources
    private static string RemoveSuffix(string name)
    {
        string[] suffixes = ["jr", "sr", "iii", "ii", "iv", "v"];

        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix))
            {
                return name.Substring(0, name.Length - suffix.Length);
            }
        }

        return name;
    }
}

