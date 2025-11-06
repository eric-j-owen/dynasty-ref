using Db;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataPipeline;

class TestDataBaseFixture
{
    private const string ConnectionString = @"Host=localhost;Database=dynastydb;Username=username;Password=password";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDataBaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using var context = CreateContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                _databaseInitialized = true;
            }
        }
    }

    public static AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
}

