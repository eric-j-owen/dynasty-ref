using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db;
using Db.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;

namespace DataPipeline.DataPipeline.Load;


public class ExternalIdsLoader(AppDbContext context) : IDataLoader<ExternalIdWithLookupDto>
{
    private readonly AppDbContext _context = context;
    public async Task<LoadResult> LoadData(List<ExternalIdWithLookupDto> data)
    {
        int addCount = 0;

        //gets sleeper ids from input data
        var sleeperIds = data.Select(r => r.SleeperId).ToHashSet();

        //matches sleeper ids to existing players
        var existingPlayers = await _context.ExternalIdPlayerLookups
            .Where(lookup => lookup.DataSource == DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId))
            .ToDictionaryAsync(lookup => lookup.SourceId, lookup => lookup.PlayerId);
        var playerIds = existingPlayers.Values.ToHashSet();

        //retrieves existing players current external ids to check duplicates
        var existingExternalIds = await _context.ExternalIdPlayerLookups
            .Where(lookup => playerIds.Contains(lookup.PlayerId))
            .Select(lookup => new { lookup.PlayerId, lookup.DataSource, lookup.SourceId })
            .ToHashSetAsync();

        foreach (var record in data)
        {
            //if player exists by sleeperid, output internal playerid
            if (existingPlayers.TryGetValue(record.SleeperId, out var internalPlayerId))
            {

                //if player's external id exists, skip record
                if (existingExternalIds.Contains(new { PlayerId = internalPlayerId, record.DataSource, record.SourceId }))
                {
                    continue;
                }

                _context.ExternalIdPlayerLookups.Add(new ExternalIdModel
                {
                    DataSource = record.DataSource,
                    SourceId = record.SourceId,
                    PlayerId = internalPlayerId,
                    Player = null!
                });

                addCount++;
            }
        }

        var res = await _context.SaveChangesAsync();
        return new LoadResult(addCount);
    }
}