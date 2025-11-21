using ClientApi.Dtos;
using Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Consts;


namespace ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(AppDbContext Context) : ControllerBase
{
    private readonly AppDbContext _context = Context;
    private static readonly HashSet<string> _validPositions = [.. Enum.GetNames<IncludedPosition>()];

    [HttpGet]
    [Route("rankings")]

    public async Task<ActionResult<IEnumerable<PlayerRankingDto>>> GetRankings(
        [FromQuery] string? searchName,
        [FromQuery] string? positions,
        [FromQuery] string? sortDataSource,
        [FromQuery] bool? isSuperFlex = true
    )
    {
        var testingDate = new DateOnly(2025, 11, 18); // only for dev 
        // var today = DateOnly.FromDateTime(DateTime.UtcNow);

        DataSource parsedDataSource = DataSource.KeepTradeCut;
        if (!string.IsNullOrEmpty(sortDataSource) && Enum.TryParse<DataSource>(sortDataSource, true, out var parsed))
        {
            parsedDataSource = parsed;
        }

        var query = _context.Players
            .Where(p => p.Values != null && p.Values.Any(v => v.CreatedAt == testingDate))
            .Include(p => p.Values)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchName.ToLower()) ||
                p.LastName.ToLower().Contains(searchName.ToLower()) ||
                p.NormalizedName.Contains(searchName.ToLower())
            );
        }

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

        var response = await query.Select(p => new PlayerRankingDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Team = p.Team,
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
        .OrderByDescending(p => p.Values.Any(v => v.Source == parsedDataSource))
        .ThenByDescending(p => p.Values.FirstOrDefault(v => v.Source == parsedDataSource)!.Value)
        .AsSplitQuery()
        .ToListAsync();

        return response;
    }
}