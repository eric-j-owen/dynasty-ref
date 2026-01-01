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
        //remove duplicate ids, need manual review due to data issue
        var uniqueIds = data
            .DistinctBy(x => new { x.DataSource, x.SourceId })
            .ToList();
        Console.WriteLine($"found {data.Count - uniqueIds.Count} duplicates");

        int addCount = 0;

        //gets sleeper ids from input data
        var sleeperIds = uniqueIds.Select(r => r.SleeperId).ToHashSet();

        //matches sleeper ids to existing players
        var existingPlayers = await _context.ExternalIdPlayerLookups
            .Where(lookup => lookup.DataSource == DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId))
            .ToDictionaryAsync(lookup => lookup.SourceId, lookup => lookup.PlayerId); //{sleeperid, playerid}

        //retrieves existing players current external ids to check duplicates
        var playerIds = existingPlayers.Values.ToHashSet();
        var existingExternalIds = await _context.ExternalIdPlayerLookups
            .Where(lookup => playerIds.Contains(lookup.PlayerId))
            .Select(lookup => new { lookup.PlayerId, lookup.DataSource, lookup.SourceId })
            .ToHashSetAsync();

        foreach (var record in uniqueIds)
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
                    PlayerId = internalPlayerId
                });

                addCount++;
            }
        }

        await _context.SaveChangesAsync();
        return new LoadResult(addCount);
    }
}