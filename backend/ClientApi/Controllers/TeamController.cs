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
        try
        {
            var res = await _espnService.GetTeamStats(teamAbbr);
            return Ok(res);
        }
        catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        catch (Exception)
        {
            return Problem(detail: "an error occured", instance: HttpContext.Request.Path);
        }

    }
}