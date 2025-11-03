using Db;
using Db.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataPipeline.Interfaces;

namespace DataPipeline.Loaders;

public class PlayerUpsertLoader(AppDbContext context) : IDataLoader<Player>
{
    private readonly AppDbContext _context = context;


    public async Task<int> LoadData(List<Player> players)
    {
        var sleeperIds = (from player in players
                          select player.ExternalIds.First().SourceId)
                         .ToHashSet();

        var existingSleeperIdLookups = await _context.ExternalIdPlayerLookups
            .Where(lookup => lookup.DataSource == Shared.Consts.DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId))
            .Include(lookup => lookup.Player)
            .ToDictionaryAsync(lookup => lookup.SourceId);

        foreach (var player in players)
        {
            var sleeperId = player.ExternalIds.First(id => id.DataSource == Shared.Consts.DataSource.Sleeper).SourceId;
            if (sleeperId != null && existingSleeperIdLookups.TryGetValue(sleeperId, out var existingPlayerLookup))
            {
                existingPlayerLookup.Player.FirstName = player.FirstName;
                existingPlayerLookup.Player.LastName = player.LastName;
                existingPlayerLookup.Player.NormalizedName = player.NormalizedName;
                existingPlayerLookup.Player.Positions = player.Positions;
                existingPlayerLookup.Player.Team = player.Team;
                existingPlayerLookup.Player.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                _context.Add(player);
            }
        }

        return await _context.SaveChangesAsync();
    }
}

