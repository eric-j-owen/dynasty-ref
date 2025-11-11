using Microsoft.EntityFrameworkCore;
using Db;
using Db.Models;
using Shared.Consts;
using DataPipeline.DataPipeline.Loaders;
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

                var player = new PlayerModel
                {
                    FirstName = "old",
                    LastName = "player",
                    NormalizedName = "oldplayer",
                    Team = TeamAbbr.CLE,
                    Positions = [IncludedPosition.QB],
                    LastUpdated = DateTime.UtcNow
                };
                player.AddExternalId(new ExternalIdModel
                {
                    DataSource = DataSource.Sleeper,
                    SourceId = "1",
                    Player = player
                });
                context.Add(player);
                context.SaveChanges();
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


public class DataPipeline(TestDataBaseFixture fixture) : IClassFixture<TestDataBaseFixture>
{
    public TestDataBaseFixture Fixture { get; } = fixture;

    [Fact]
    public async Task UpsertPlayer_NewPlayer_AddsToDb()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var initialCount = context.Players.Count();

        var player = new PlayerModel
        {
            FirstName = "new",
            LastName = "player",
            NormalizedName = "newplayer",
            Team = TeamAbbr.CLE,
            Positions = [IncludedPosition.QB],
            LastUpdated = DateTime.UtcNow
        };

        player.AddExternalId(
            new ExternalIdModel
            {
                DataSource = DataSource.Sleeper,
                SourceId = "2",
                Player = player
            }
        );

        var loader = new PlayerUpsertLoader(context, NullLogger<PlayerUpsertLoader>.Instance);
        await loader.LoadData([player]);

        context.ChangeTracker.Clear();

        Assert.Equal(initialCount + 1, context.Players.Count());
        Assert.Equal(1, loader.AddCount);
        Assert.Equal(0, loader.UpdateCount);

    }

    [Fact]
    public async Task UpsertPlayer_ExistingPlayer_updates()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();
        var loader = new PlayerUpsertLoader(context, NullLogger<PlayerUpsertLoader>.Instance);

        var updatedPlayer = new PlayerModel
        {

            FirstName = "updated",
            LastName = "player",
            NormalizedName = "updatedplayer",
            Team = TeamAbbr.CLE,
            Positions = [IncludedPosition.QB, IncludedPosition.TE],
            LastUpdated = DateTime.UtcNow
        };
        updatedPlayer.AddExternalId(new ExternalIdModel
        {
            DataSource = DataSource.Sleeper,
            SourceId = "1",
            Player = updatedPlayer
        });

        await loader.LoadData([updatedPlayer]);
        context.ChangeTracker.Clear();
        var updated = await context.Players
            .Include(p => p.ExternalIds)
            .SingleAsync(p => p.LastName == "player", TestContext.Current.CancellationToken);

        Assert.Equal(updatedPlayer.FirstName, updated.FirstName);
        Assert.Equal(updatedPlayer.Positions, updated.Positions);
        Assert.Single(context.Players);
        Assert.Single(context.Players.First().ExternalIds);
        Assert.Equal(1, loader.UpdateCount);
        Assert.Equal(0, loader.AddCount);

    }

    [Fact]
    public async Task UpsertPlayer_ExistingPlayer_noUpdatesIsProperlyHandled()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();
        var loader = new PlayerUpsertLoader(context, NullLogger<PlayerUpsertLoader>.Instance);

        var p = context.Players.Include(p => p.ExternalIds).First();
        var samePlayer = new PlayerModel
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            NormalizedName = p.NormalizedName,
            Team = p.Team,
            Positions = p.Positions,
            LastUpdated = p.LastUpdated
        };
        samePlayer.AddExternalId(new ExternalIdModel
        {
            DataSource = p.ExternalIds.First().DataSource,
            SourceId = p.ExternalIds.First().SourceId,
            Player = samePlayer
        });

        await loader.LoadData([samePlayer]);

        Assert.Equal(0, loader.UpdateCount);
        Assert.Equal(0, loader.AddCount);
    }

}
