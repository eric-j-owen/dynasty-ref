using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data;
using Data.Models;
using Scrapers.Services;

public class DbService
{

    private readonly string _connectionString;
    public DbService()
    {
        //user-secrets init
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        _connectionString = config["ConnectionStrings:AppDbContext"];
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new AppDbContext(options);
    }
}