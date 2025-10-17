using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data.Models;

namespace Data.Services;
public class PlayerDbService
{
    private readonly AppDbContext _context;

    public PlayerDbService(AppDbContext context)
    {
        _context = context;
    }

    public void ProcessPlayersFromFile(string filePath)
    {
        //check file 
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
            using var transaction = _context.Database.BeginTransaction();

            //truncate staging table
            _context.Database.ExecuteSqlRaw("truncate table \"PlayersStaging\";");

            //insert to staging
            _context.PlayersStaging.AddRange(players.Values);
            _context.SaveChanges();

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

            var rows = _context.Database.ExecuteSqlRaw(q);
            transaction.Commit();
            Console.WriteLine($"{rows} row(s) affected");
        }
        catch (Exception e)
        {
            throw new Exception($"Db transaction failed:{e}");
        }
    }   
}