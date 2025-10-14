using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data;
using Data.Models;

namespace Scrapers.Services;

public class DbService
{
    private readonly string _connectionString;
    public DbService()
    {
        //user-secrets init
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        _connectionString = config["ConnectionStrings:AppDbContext"]
            ?? throw new Exception("no connection string found");
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new AppDbContext(options);
    }

    public async Task Main()
    {
        try
        {
            // get files
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


            //deserialize
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }


            //db operations

        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e}");
        }
    }
}