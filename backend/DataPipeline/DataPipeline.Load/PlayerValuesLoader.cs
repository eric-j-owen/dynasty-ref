using DataPipeline.DTOs;
using DataPipeline.Interfaces;
using Db;
using Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Shared.Consts;

namespace DataPipeline.DataPipeline.Load;

public class PlayerValuesLoader(AppDbContext context) : IDataLoader<PlayerValueWithLookupDto>
{
    private readonly AppDbContext _context = context;
    public async Task<LoadResult> LoadData(List<PlayerValueWithLookupDto> data)
    {
        int addCount = 0;

        //cleanup old values
        var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3));
        var deletedCount = await _context.PlayerValues
            .Where(value => value.CreatedAt < cutoffDate)
            .ExecuteDeleteAsync();

        //lookup ids from incoming data, used for mapping to existing players
        var sleeperIds = data
            .Where(player => player.LookupIds.ContainsKey(DataSource.Sleeper))
            .Select(player => player.LookupIds[DataSource.Sleeper])
            .Distinct()
            .ToHashSet();
        var ktcIds = data
            .Where(player => player.LookupIds.ContainsKey(DataSource.KeepTradeCut))
            .Select(player => player.LookupIds[DataSource.KeepTradeCut])
            .Distinct()
            .ToHashSet();
        var mflIds = data
            .Where(player => player.LookupIds.ContainsKey(DataSource.Mfl))
            .Select(player => player.LookupIds[DataSource.Mfl])
            .Distinct()
            .ToHashSet();

        //map to existing players, output dict {(datasource, id), internalplayerid}
        var playerMappings = await _context.ExternalIdPlayerLookups
            .Where(lookup => (
                lookup.DataSource == DataSource.Sleeper && sleeperIds.Contains(lookup.SourceId) ||
                lookup.DataSource == DataSource.KeepTradeCut && ktcIds.Contains(lookup.SourceId) ||
                lookup.DataSource == DataSource.Mfl && mflIds.Contains(lookup.SourceId)
            ))
            .ToDictionaryAsync(lookup => (lookup.DataSource, lookup.SourceId), lookup => lookup.PlayerId);



        foreach (var record in data)
        {
            int? playerId = null;
            foreach (var (source, sourceId) in record.LookupIds)
            {
                if (playerMappings.TryGetValue((source, sourceId), out var foundPlayerId))
                {
                    playerId = foundPlayerId;
                }
            }

            if (!playerId.HasValue) continue;

            if (record.OneQbValue.HasValue)
            {
                _context.PlayerValues.Add(new PlayerValueModel
                {
                    PlayerId = playerId.Value,
                    DataSource = record.ValueSource,
                    Value = record.OneQbValue.Value,
                    IsSuperFlex = false,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                });

                addCount++;
            }

            if (record.SuperFlexValue.HasValue)
            {
                _context.PlayerValues.Add(new PlayerValueModel
                {
                    PlayerId = playerId.Value,
                    DataSource = record.ValueSource,
                    Value = record.SuperFlexValue.Value,
                    IsSuperFlex = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                });

                addCount++;
            }
        }
        await _context.SaveChangesAsync();
        return new LoadResult(addCount, null, deletedCount);
    }
}