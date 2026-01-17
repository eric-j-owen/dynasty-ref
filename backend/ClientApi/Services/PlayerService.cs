using ClientApi.Dtos;
using ClientApi.Dtos.Players;
using Db;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;

namespace ClientApi.Services;

public class PlayerService(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    private static readonly HashSet<string> _validPositions = [.. Enum.GetNames<IncludedPosition>()];

    public async Task<PaginatedResults<PlayerRankingDto>> GetPlayerRankings(
        string? searchName,
        string? positions,
        bool isSuperFlex,
        DataSource sortDataSource,
        int page,
        int pageSize
    )
    {
        var testingDate = new DateOnly(2026, 1, 10); // only for dev 
        // var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //initial query to get all player values
        var query = _context.Players
            .Where(p => p.Values != null && p.Values.Any(v => v.CreatedAt == testingDate))
            .Include(p => p.Values)
            .AsQueryable();

        //search filter
        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchName.ToLower()) ||
                p.LastName.ToLower().Contains(searchName.ToLower()) ||
                p.NormalizedName.Contains(searchName.ToLower())
            );
        }

        // position filter
        if (!string.IsNullOrEmpty(positions))
        {
            var filteredPositions = positions
                .Split(",")
                .Select(pos => pos.Trim().ToUpper())
                .Where(_validPositions.Contains)
                .Select(Enum.Parse<IncludedPosition>)
                .ToList();

            if (filteredPositions.Count != 0)
            {
                query = query.Where(p =>
                    p.Positions.Any(pos => filteredPositions.Contains(pos)));
            }

        }

        //final mapping and order filter
        var items = await query.Select(p => new PlayerRankingDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Positions = p.Positions,
            LastUpdated = p.LastUpdated,
            Values = p.Values!
                .Where(v => v.IsSuperFlex == isSuperFlex)
                .Select(v => new PlayerValueDto
                {
                    IsSuperFlex = v.IsSuperFlex,
                    Value = v.Value,
                    Source = v.DataSource
                }).ToList()
        })
        .OrderByDescending(p => p.Values.Any(v => v.Source == sortDataSource))
        .ThenByDescending(p => p.Values.FirstOrDefault(v => v.Source == sortDataSource)!.Value)
        .Skip(page * pageSize)
        .Take(pageSize)
        .AsSplitQuery()
        .ToListAsync();

        return new PaginatedResults<PlayerRankingDto>
        (
            page,
            pageSize,
            items
        );
    }

    // public async Task<PlayerDetailsDto> GetPlayerDetails(int playerId)
    // {
    //     var player = await _context.Players
    //         .Where(p => p.Id == playerId)
    // }

}