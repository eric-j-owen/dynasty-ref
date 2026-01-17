using ClientApi.Dtos.Espn;
using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamController(EspnService espnService) : ControllerBase
{
    private readonly EspnService _espnService = espnService;

    [HttpGet]
    [Route("stats/{teamAbbr}")]
    public async Task<ActionResult<MappedTeamStats>> GetTeamStats(string teamAbbr)
    {
        var res = await _espnService.GetTeamStats(teamAbbr);
        if (res == null)
        {
            return NotFound();
        }

        return Ok(res);
    }
}