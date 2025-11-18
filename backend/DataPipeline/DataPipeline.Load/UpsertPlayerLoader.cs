using Db;
using Db.Models;
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

        var sleeperIds = (from player in transformedSleeperPlayers
                          select player.ExternalIds.First().SourceId)
                         .ToHashSet();

        var existingSleeperIdLookups = await _context.ExternalIdPlayerLookups
            .Where(lookup => lookup.DataSource == DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId))
            .Include(lookup => lookup.Player)
            .ToDictionaryAsync(lookup => lookup.SourceId);


        foreach (var currPlayer in transformedSleeperPlayers)
        {
            var sleeperId = currPlayer.ExternalIds.First(id => id.DataSource == DataSource.Sleeper).SourceId;

            //if existing by sleeperid, output entity
            if (existingSleeperIdLookups.TryGetValue(sleeperId, out var existingPlayerLookup))
            {
                if (existingPlayerLookup.Player == null)
                {
                    throw new Exception($"navigation property not set for {existingPlayerLookup.PlayerId}");
                }

                bool isChanged = CheckIsChanged(existingPlayerLookup.Player, currPlayer);

                if (isChanged)
                {
                    PerformUpdate(existingPlayerLookup.Player, currPlayer);
                    updateCount++;
                }
            }
            else
            {
                _context.Add(currPlayer);
                addCount++;
            }
        }

        await _context.SaveChangesAsync();
        return new LoadResult(addCount, updateCount);
    }


    static private bool CheckIsChanged(PlayerModel oldData, PlayerModel newData)
    {
        return (
            !oldData.FirstName.Equals(newData.FirstName) ||
            !oldData.LastName.Equals(newData.LastName) ||
            !oldData.NormalizedName.Equals(newData.NormalizedName) ||
            !oldData.Team.Equals(newData.Team) ||
            !oldData.Positions.SequenceEqual(newData.Positions)
        );
    }

    static private void PerformUpdate(PlayerModel oldData, PlayerModel newData)
    {
        oldData.FirstName = newData.FirstName;
        oldData.LastName = newData.LastName;
        oldData.NormalizedName = newData.NormalizedName;
        oldData.Positions = newData.Positions;
        oldData.Team = newData.Team;
        oldData.LastUpdated = DateTime.UtcNow;
    }
}

