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
    throw new ArgumentException("missing argument");
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
        await SaveToDbAsync(options);
    }

    //run everything
    else if (args.Contains("--all"))
    {
        var players = await FetchPlayersAsync(client);
        await WritePlayersJsonAsync(players);
        await SaveToDbAsync(options);
    }

    else
    {
        throw new ArgumentException("invalid argument");
    }

}

/*
----------------
methods
----------------
*/

static async Task<Dictionary<string, PlayerStaging>> FetchPlayersAsync(HttpClient client)
{
    try
    {
        string url = "https://api.sleeper.app/v1/players/nfl";
        var players = await client.GetFromJsonAsync<Dictionary<string, PlayerStaging>>(url);
   
        Console.WriteLine("fetched players");

        return players ?? new();
    }
    catch (Exception e)
    {
        Console.WriteLine($"error: {e}");
        throw;
    }
   
}

static async Task WritePlayersJsonAsync(Dictionary<string, PlayerStaging> players)
{
    try
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
        await using FileStream createStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(createStream, players);

        Console.WriteLine("saved players.json");
    }
    catch (Exception e)
    {
        Console.WriteLine($"error: {e}");
        throw;
    }
}

static async Task SaveToDbAsync(DbContextOptions<AppDbContext> options)
{
    try
    {
        //check file 
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "players.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("file not found");
            return;
        }

        //read file
        string json = await File.ReadAllTextAsync(filePath);

        //deserialize
        var players = JsonSerializer.Deserialize<Dictionary<string, PlayerStaging>>(json);


        //db context
        using var ctx = new AppDbContext(options);

        //truncate staging table
        await ctx.Database.ExecuteSqlRawAsync("truncate table \"PlayersStaging\";");

        //insert to staging
        if (players == null)
        {
            Console.WriteLine("null players dict");
            return;
        }

        ctx.PlayersStaging.AddRange(players.Values);
        await ctx.SaveChangesAsync();

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
            ) THEN
                UPDATE SET
                    ""FirstName"" = source.""FirstName"",
                    ""LastName"" = source.""LastName"",
                    ""Team"" = source.""Team"",
                    ""Position"" = source.""Position"",
                    ""FantasyPositions"" = source.""FantasyPositions"",
                    ""Status"" = source.""Status"",
                    ""InjuryStatus"" = source.""InjuryStatus"",
                    ""LastUpdated"" = source.""LastUpdated""
            WHEN NOT MATCHED THEN
                INSERT 
                (
                    ""PlayerId"", ""FirstName"", ""LastName"", ""Team"", ""Position"", 
                    ""FantasyPositions"", ""Status"", ""InjuryStatus"", ""LastUpdated"" 
                )
                VALUES  
                (
                    source.""PlayerId"", source.""FirstName"", source.""LastName"", source.""Team"", source.""Position"", 
                    source.""FantasyPositions"", source.""Status"", source.""InjuryStatus"", source.""LastUpdated"" 
                )  
            
        ;";

        var rows = await ctx.Database.ExecuteSqlRawAsync(q);
        Console.WriteLine($"{rows} row(s) affected");

    }
    catch (Exception e)
    {
        Console.WriteLine($"error: {e}");
        throw;
    }
    
}

