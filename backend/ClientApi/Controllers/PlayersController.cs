using ClientApi.Dtos;
using Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(AppDbContext Context) : ControllerBase
{
    private readonly AppDbContext _context = Context;

    [HttpGet]
    [Route("rankings")]

    public async Task<ActionResult<IEnumerable<PlayerRankingDto>>> GetRankings(
        [FromQuery] string search,
        [FromQuery] bool isSuperFlex,
        [FromQuery] string positions
    )
    {
        var testingDate = new DateOnly(2025, 11, 18); // only for dev 
        // var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.Players
            .Where(p => p.Values != null && p.Values.Any(v => v.CreatedAt == testingDate))
            .Include(p => p.Values)
            .AsSplitQuery()
            .Select(p => new PlayerRankingDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Team = p.Team,
                Positions = p.Positions,
                LastUpdated = p.LastUpdated,
                Values = p.Values!
                    .Where(v => v.IsSuperFlex == true)
                    .Select(v => new PlayerValueDto
                    {
                        IsSuperFlex = v.IsSuperFlex,
                        Value = v.Value,
                        Source = v.DataSource
                    }).ToList()
            }).ToListAsync();
    }
}