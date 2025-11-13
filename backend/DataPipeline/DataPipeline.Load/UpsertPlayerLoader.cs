using Db;
using Db.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DataPipeline.Interfaces;
using Shared.Consts;

namespace DataPipeline.DataPipeline.Load;

public class PlayerUpsertLoader(AppDbContext context) : IDataLoader<PlayerModel>
{
    private readonly AppDbContext _context = context;

    public async Task<LoadResult> LoadData(List<PlayerModel> transformedSleeperPlayers)
    {
        int addCount = 0; int updateCount = 0;

        // gets sleeperids from transformed data coming from sleeper api 
        var sleeperIds = (from player in transformedSleeperPlayers
                          select player.ExternalIds.First().SourceId)
                         .ToHashSet();

        // lookups players in database that have that id
        var existingSleeperIdLookups = await _context.ExternalIdPlayerLookups
            .Where(lookup => lookup.DataSource == DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId))
            .Include(lookup => lookup.Player)
            .ToDictionaryAsync(lookup => lookup.SourceId);


        foreach (var player in transformedSleeperPlayers)
        {
            //current players sleeper id
            var sleeperId = player.ExternalIds.First(id => id.DataSource == DataSource.Sleeper).SourceId;

            //if existing by sleeperid, output entity
            if (existingSleeperIdLookups.TryGetValue(sleeperId, out var existingPlayerLookup))
            {
                var existingPlayer = existingPlayerLookup.Player;

                //check for any changes before updating
                bool isChanged =
                    !existingPlayer.FirstName.Equals(player.FirstName) ||
                    !existingPlayer.LastName.Equals(player.LastName) ||
                    !existingPlayer.NormalizedName.Equals(player.NormalizedName) ||
                    !existingPlayer.Team.Equals(player.Team) ||
                    !existingPlayer.Positions.SequenceEqual(player.Positions);

                if (isChanged)
                {
                    existingPlayer.FirstName = player.FirstName;
                    existingPlayer.LastName = player.LastName;
                    existingPlayer.NormalizedName = player.NormalizedName;
                    existingPlayer.Positions = player.Positions;
                    existingPlayer.Team = player.Team;
                    existingPlayer.LastUpdated = DateTime.UtcNow;

                    updateCount++;
                }
            }
            else
            {
                _context.Add(player);
                addCount++;
            }
        }

        await _context.SaveChangesAsync();
        return new LoadResult(addCount, updateCount);
    }
}

