using Microsoft.EntityFrameworkCore;
using Db;
using Db.Models;
using Shared.Consts;
using DataPipeline.Loaders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.DataPipeline;

public class TestDataBaseFixture
{
    private const string ConnectionString = @"Host=localhost;Database=test_dynasty_db;Username=username;Password=password";

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

                var player = new Player
                {
                    FirstName = "old",
                    LastName = "player",
                    NormalizedName = "oldplayer",
                    Team = TeamAbbr.CLE,
                    Positions = [IncludedPosition.QB],
                    LastUpdated = DateTime.UtcNow
                };
                player.AddExternalId(new ExternalIdPlayerLookup
                {
                    DataSource = DataSource.Sleeper,
                    SourceId = "1",
                    Player = player
                });
                context.Add(player);
            }
            _databaseInitialized = true;
        }
    }

    public AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
}


public class SleeperPipelineTests(TestDataBaseFixture fixture) : IClassFixture<TestDataBaseFixture>
{
    public TestDataBaseFixture Fixture { get; } = fixture;


    [Fact]
    public async Task LoadData_NewPlayer_AddsToDb()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var player = new Player
        {
            FirstName = "test",
            LastName = "player",
            NormalizedName = "testplayer",
            Team = TeamAbbr.CLE,
            Positions = [IncludedPosition.QB],
            LastUpdated = DateTime.UtcNow
        };

        player.AddExternalId(
            new ExternalIdPlayerLookup
            {
                DataSource = DataSource.Sleeper,
                SourceId = "1",
                Player = player
            }
        );

        var loader = new PlayerUpsertLoader(context, NullLogger<PlayerUpsertLoader>.Instance);
        await loader.LoadData(new List<Player> { player });

        context.ChangeTracker.Clear();

        var saved = await context.Players.SingleAsync(p => p.LastName == "player", TestContext.Current.CancellationToken);

        Assert.Equal("test", saved.FirstName);
    }

    [Fact]
    public async Task LoadData_ExistingPlayer_updates()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();
        var loader = new PlayerUpsertLoader(context, NullLogger<PlayerUpsertLoader>.Instance);

        var updatedPlayer = new Player
        {

            FirstName = "new",
            LastName = "player",
            NormalizedName = "newplayer",
            Team = TeamAbbr.CLE,
            Positions = [IncludedPosition.QB],
            LastUpdated = DateTime.UtcNow
        };
        updatedPlayer.AddExternalId(new ExternalIdPlayerLookup
        {
            DataSource = DataSource.Sleeper,
            SourceId = "1",
            Player = updatedPlayer
        });

        await loader.LoadData(new List<Player> { updatedPlayer });
        context.ChangeTracker.Clear();
        var updated = await context.Players
            .Include(p => p.ExternalIds)
            .SingleAsync(p => p.LastName == "player", TestContext.Current.CancellationToken);

        Assert.Equal("new", updated.FirstName);
        Assert.Single(context.Players);
        Assert.Single(context.Players.First().ExternalIds);
    }

}
