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
try
{
    if (args.Length == 0)
    {
        throw new Exception("missing argument: --push --pull --all");
    }
    else
    {
        //fetch all players and save locally
        if (args.Contains("--pull"))
        {
            var players = await FetchPlayersAsync(client);
            WritePlayersJson(players);
        }

        //update db with players.json
        else if (args.Contains("--push"))
        {
            SaveToDb(options);
        }

        //run everything
        else if (args.Contains("--all"))
        {
            var players = await FetchPlayersAsync(client);
            WritePlayersJson(players);
            SaveToDb(options);
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

/*
----------------
methods
----------------
*/

static async Task<Dictionary<string, PlayerStaging>> FetchPlayersAsync(HttpClient client)
{
   
    string url = "https://api.sleeper.app/v1/players/nfl";
    var players = await client.GetFromJsonAsync<Dictionary<string, PlayerStaging>>(url);

    Console.WriteLine("fetched players");

    if (players == null)
    {
        throw new Exception("failed fetch: sleeper api returned null players dict");
    }

    return players;
}

static void WritePlayersJson(Dictionary<string, PlayerStaging> players)
{
    string fileName = "players.json";
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
    string jsonStr = JsonSerializer.Serialize(players);
    File.WriteAllText(filePath, jsonStr);

    Console.WriteLine("saved players.json");
}

static void SaveToDb(DbContextOptions<AppDbContext> options)
{
    //check file 
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
    if (!File.Exists(filePath))
    {
        throw new Exception($"file not found at path {filePath}");
    }

    //read file
    string jsonStr = File.ReadAllText(filePath);

    //deserialize
    var players = JsonSerializer.Deserialize<Dictionary<string, PlayerStaging>>(jsonStr);
    if (players == null)
    {
        throw new Exception("failed deserialize players.json");
    }

    //db operations
    try
    {
        //db context and transaction init
        using var ctx = new AppDbContext(options);
        using var transaction = ctx.Database.BeginTransaction();

        //truncate staging table
        ctx.Database.ExecuteSqlRaw("truncate table \"PlayersStaging\";");

        //insert to staging
        ctx.PlayersStaging.AddRange(players.Values);
        ctx.SaveChanges();

        //upsert with players table
        string q =
        @"
            MERGE INTO ""Players"" AS target
            USING ""PlayersStaging"" AS source 
            ON target.""PlayerId"" = source.""PlayerId""
            WHEN MATCHED AND (
                target.""FirstName"" IS DISTINCT FROM source.""FirstName""
                OR target.""LastName"" IS DISTINCT FROM source.""LastName""
                OR target.""Team"" IS DISTINCT FROM source.""Team""
                OR target.""Position"" IS DISTINCT FROM source.""Position""
                OR target.""FantasyPositions"" IS DISTINCT FROM source.""FantasyPositions""
                OR target.""Status"" IS DISTINCT FROM source.""Status""
                OR target.""InjuryStatus"" IS DISTINCT FROM source.""InjuryStatus""
                OR target.""SearchFullName"" IS DISTINCT FROM source.""SearchFullName""
            ) THEN
                UPDATE SET
                    ""FirstName"" = source.""FirstName"",
                    ""LastName"" = source.""LastName"",
                    ""Team"" = source.""Team"",
                    ""Position"" = source.""Position"",
                    ""FantasyPositions"" = source.""FantasyPositions"",
                    ""Status"" = source.""Status"",
                    ""InjuryStatus"" = source.""InjuryStatus"",
                    ""SearchFullName"" = source.""SearchFullName"",
                    ""LastUpdated"" = source.""LastUpdated""
            WHEN NOT MATCHED THEN
                INSERT 
                (
                    ""PlayerId"", ""FirstName"", ""LastName"", ""Team"", ""Position"", 
                    ""FantasyPositions"", ""Status"", ""InjuryStatus"", ""SearchFullName"", ""LastUpdated"" 
                )
                VALUES  
                (
                    source.""PlayerId"", source.""FirstName"", source.""LastName"", source.""Team"", source.""Position"", 
                    source.""FantasyPositions"", source.""Status"", source.""InjuryStatus"", source.""SearchFullName"", source.""LastUpdated"" 
                )
        ;";

        var rows = ctx.Database.ExecuteSqlRaw(q);
        transaction.Commit();
        Console.WriteLine($"{rows} row(s) affected");
    }
    catch (Exception e)
    {
        throw new Exception($"Db transaction failed: {e}");
    }
}

